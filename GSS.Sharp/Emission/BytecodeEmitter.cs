using GSS.Core.ApiBinding.Models;
using GSS.Core.Compiler.Emission;
using GSS.Core.Runtime.Execution;
using GSS.Sharp.Binding.Nodes;
using GSS.Sharp.Binding.Nodes.Expressions;
using GSS.Sharp.Binding.Nodes.Statements;
using GSS.Sharp.Binding.Symbols;

namespace GSS.Sharp.Emission
{
	public sealed class BytecodeEmitter
	{
		private readonly List<Instruction> _instructions = new();
		private readonly Dictionary<BoundLabel, int> _labelOffsets = new();
		private readonly List<(int instIndex, BoundLabel target, bool isBranch, bool jumpIfTrue)> _jumpsToPatch = new();

		private readonly Dictionary<GssValue, int> _constantsPoolMap = new(new GssValueComparer());
		private readonly List<GssValue> _constantsPool = new();

		private readonly Dictionary<object, int> _metadataMap = new();
		private readonly List<object> _metadataPool = new();

		private RegisterAllocator _allocator = null!;

		public ExecutableGraph Emit(BoundBlockStatement root, int variableCount)
		{
			_allocator = new RegisterAllocator(variableCount);

			for (int i = 0; i < root.Statements.Count; i++)
			{
				EmitStatement(root.Statements[i]);
			}

			EmitInstruction(new Instruction(OpCode.Halt));

			foreach (var patch in _jumpsToPatch)
			{
				int targetOffset = _labelOffsets[patch.target];
				if (patch.isBranch)
				{
					int trueJump = patch.jumpIfTrue ? targetOffset : patch.instIndex + 1;
					int falseJump = patch.jumpIfTrue ? patch.instIndex + 1 : targetOffset;
					_instructions[patch.instIndex] = new Instruction(patch.jumpIfTrue ? OpCode.BranchTrue : OpCode.BranchFalse, trueJump, falseJump);
				}
				else
				{
					_instructions[patch.instIndex] = new Instruction(OpCode.Jump, targetOffset);
				}
			}

			return new ExecutableGraph(_instructions.ToArray(), _constantsPool.ToArray(), _metadataPool.ToArray(), _allocator.GetTotalRequiredRegisters());
		}

		private void EmitInstruction(Instruction inst) => _instructions.Add(inst);
		private void MarkLabel(BoundLabel label) => _labelOffsets[label] = _instructions.Count;

		private void EmitJump(BoundLabel target)
		{
			_jumpsToPatch.Add((_instructions.Count, target, false, false));
			EmitInstruction(new Instruction(OpCode.Nop));
		}

		private void EmitConditionalJump(BoundLabel target, bool jumpIfTrue)
		{
			_jumpsToPatch.Add((_instructions.Count, target, true, jumpIfTrue));
			EmitInstruction(new Instruction(OpCode.Nop));
		}

		private int GetOrAddMetadata(object obj)
		{
			if (!_metadataMap.TryGetValue(obj, out int index))
			{
				index = _metadataPool.Count;
				_metadataPool.Add(obj);
				_metadataMap[obj] = index;
			}
			return index;
		}

		private void EmitStatement(BoundStatement node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.ExpressionStatement:
					EmitExpression(((BoundExpressionStatement)node).Expression);
					break;
				case BoundNodeKind.LabelStatement:
					MarkLabel(((BoundLabelStatement)node).Label);
					break;
				case BoundNodeKind.GotoStatement:
					EmitJump(((BoundGotoStatement)node).Label);
					break;
				case BoundNodeKind.ConditionalGotoStatement:
					var cgs = (BoundConditionalGotoStatement)node;
					EmitExpression(cgs.Condition);
					EmitConditionalJump(cgs.Label, cgs.JumpIfTrue);
					break;
				case BoundNodeKind.VariableDeclaration:
					var decl = (BoundVariableDeclaration)node;
					EmitExpression(decl.Initializer);
					EmitInstruction(new Instruction(OpCode.StoreReg, decl.Variable.RegisterIndex));
					break;
				case BoundNodeKind.WaitStatement:
					EmitExpression(((BoundWaitStatement)node).Duration);
					EmitInstruction(new Instruction(OpCode.Wait));
					break;
				case BoundNodeKind.ReturnStatement:
					var ret = (BoundReturnStatement)node;
					if (ret.Expression != null) EmitExpression(ret.Expression);
					EmitInstruction(new Instruction(OpCode.Halt));
					break;
			}
		}

		private void EmitExpression(BoundExpression node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.LiteralExpression:
					var lit = (BoundLiteralExpression)node;
					var val = lit.Type == TypeSymbol.Int ? GssValue.FromInt((int)lit.Value!) :
							  lit.Type == TypeSymbol.Float ? GssValue.FromFloat((float)lit.Value!) :
							  lit.Type == TypeSymbol.Bool ? GssValue.FromBool((bool)lit.Value!) : GssValue.FromObject(lit.Value);

					if (!_constantsPoolMap.TryGetValue(val, out int cIdx))
					{
						cIdx = _constantsPool.Count;
						_constantsPool.Add(val);
						_constantsPoolMap[val] = cIdx;
					}
					EmitInstruction(new Instruction(OpCode.LoadConst, cIdx));
					break;

				case BoundNodeKind.VariableExpression:
					EmitInstruction(new Instruction(OpCode.LoadReg, ((BoundVariableExpression)node).Variable.RegisterIndex));
					break;

				case BoundNodeKind.AssignmentExpression:
					var assign = (BoundAssignmentExpression)node;
					EmitExpression(assign.Expression);
					EmitInstruction(new Instruction(OpCode.StoreReg, assign.Variable.RegisterIndex));
					break;

				case BoundNodeKind.ApiAssignmentExpression:
					var apiSet = (BoundApiAssignmentExpression)node;
					EmitExpression(apiSet.Expression);
					EmitInstruction(new Instruction(OpCode.StoreProp, GetOrAddMetadata(apiSet.Property)));
					break;

				case BoundNodeKind.CallExpression:
					var call = (BoundCallExpression)node;
					int[] args = new int[call.Arguments.Count];
					for (int i = 0; i < call.Arguments.Count; i++)
					{
						EmitExpression(call.Arguments[i]);
						int reg = _allocator.AllocateTemp();
						EmitInstruction(new Instruction(OpCode.StoreReg, reg));
						args[i] = reg;
					}

					for (int i = 0; i < args.Length; i++)
					{
						EmitInstruction(new Instruction(OpCode.PushArg, args[i], i));
						_allocator.FreeTemp(args[i]);
					}

					EmitInstruction(new Instruction(OpCode.CallMethod, GetOrAddMetadata(call.Method), args.Length));
					break;

				case BoundNodeKind.IndexExpression:
					var idx = (BoundIndexExpression)node;
					EmitExpression(idx.Collection);
					int colReg = _allocator.AllocateTemp();
					EmitInstruction(new Instruction(OpCode.StoreReg, colReg));
					EmitExpression(idx.Index);
					EmitInstruction(new Instruction(OpCode.LoadElement, colReg));
					_allocator.FreeTemp(colReg);
					break;

				case BoundNodeKind.IndexAssignmentExpression:
					var idxAss = (BoundIndexAssignmentExpression)node;
					EmitExpression(idxAss.Collection);
					int cr = _allocator.AllocateTemp();
					EmitInstruction(new Instruction(OpCode.StoreReg, cr));
					EmitExpression(idxAss.Index);
					int ir = _allocator.AllocateTemp();
					EmitInstruction(new Instruction(OpCode.StoreReg, ir));
					EmitExpression(idxAss.Expression);
					EmitInstruction(new Instruction(OpCode.StoreElement, cr, ir));
					_allocator.FreeTemp(ir);
					_allocator.FreeTemp(cr);
					break;

				case BoundNodeKind.ConversionExpression:
					var conv = (BoundConversionExpression)node;
					EmitExpression(conv.Expression);
					OpCode castOp = conv.Type == TypeSymbol.Float ? OpCode.CastIntToFloat :
									conv.Type == TypeSymbol.Int ? OpCode.CastFloatToInt : OpCode.CastAnyToString;
					EmitInstruction(new Instruction(castOp));
					break;

				case BoundNodeKind.UnaryExpression:
					var unary = (BoundUnaryExpression)node;
					EmitExpression(unary.Operand);
					OpCode unOp = unary.Op.Kind switch
					{
						BoundUnaryOperatorKind.Negation => unary.Type == TypeSymbol.Int ? OpCode.NegateInt : OpCode.NegateFloat,
						BoundUnaryOperatorKind.LogicalNegation => OpCode.NotBool,
						BoundUnaryOperatorKind.BitwiseNegation => OpCode.BitNotInt,
						_ => OpCode.Nop
					};
					EmitInstruction(new Instruction(unOp));
					break;

				case BoundNodeKind.BinaryExpression:
					var bin = (BoundBinaryExpression)node;

					if (bin.Op.Kind == BoundBinaryOperatorKind.LogicalAnd)
					{
						EmitExpression(bin.Left);
						int patchIdx = _instructions.Count;
						EmitInstruction(new Instruction(OpCode.Nop));
						EmitExpression(bin.Right);
						_instructions[patchIdx] = new Instruction(OpCode.BranchFalse, _instructions.Count, patchIdx + 1);
						return;
					}

					if (bin.Op.Kind == BoundBinaryOperatorKind.LogicalOr)
					{
						EmitExpression(bin.Left);
						int patchIdx = _instructions.Count;
						EmitInstruction(new Instruction(OpCode.Nop));
						EmitExpression(bin.Right);
						_instructions[patchIdx] = new Instruction(OpCode.BranchTrue, _instructions.Count, patchIdx + 1);
						return;
					}

					EmitExpression(bin.Right);
					int rReg = _allocator.AllocateTemp();
					EmitInstruction(new Instruction(OpCode.StoreReg, rReg));
					EmitExpression(bin.Left);

					OpCode op = bin.Op.Kind switch
					{
						BoundBinaryOperatorKind.Addition => bin.Type == TypeSymbol.Int ? OpCode.AddInt : (bin.Type == TypeSymbol.Float ? OpCode.AddFloat : OpCode.AddString),
						BoundBinaryOperatorKind.Subtraction => bin.Type == TypeSymbol.Int ? OpCode.SubInt : OpCode.SubFloat,
						BoundBinaryOperatorKind.Multiplication => bin.Type == TypeSymbol.Int ? OpCode.MulInt : OpCode.MulFloat,
						BoundBinaryOperatorKind.Division => bin.Type == TypeSymbol.Int ? OpCode.DivInt : OpCode.DivFloat,
						BoundBinaryOperatorKind.Equals => bin.Left.Type == TypeSymbol.Int ? OpCode.EqInt : OpCode.EqFloat,
						BoundBinaryOperatorKind.NotEquals => bin.Left.Type == TypeSymbol.Int ? OpCode.NeqInt : OpCode.NeqFloat,
						BoundBinaryOperatorKind.Less => bin.Left.Type == TypeSymbol.Int ? OpCode.LtInt : OpCode.LtFloat,
						BoundBinaryOperatorKind.Greater => bin.Left.Type == TypeSymbol.Int ? OpCode.GtInt : OpCode.GtFloat,
						BoundBinaryOperatorKind.LessOrEquals => bin.Left.Type == TypeSymbol.Int ? OpCode.LteInt : OpCode.LteFloat,
						BoundBinaryOperatorKind.GreaterOrEquals => bin.Left.Type == TypeSymbol.Int ? OpCode.GteInt : OpCode.GteFloat,
						_ => throw new NotImplementedException()
					};

					EmitInstruction(new Instruction(op, rReg));
					_allocator.FreeTemp(rReg);
					break;

				case BoundNodeKind.ApiPropertyExpression:
					EmitInstruction(new Instruction(OpCode.LoadProp, GetOrAddMetadata(((BoundApiPropertyExpression)node).Property)));
					break;
			}
		}
	}
}