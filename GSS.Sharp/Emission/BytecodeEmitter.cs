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

		private struct JumpPatch { public int InstIndex; public BoundLabel Target; public bool IsBranch; public bool JumpIfTrue; }
		private JumpPatch[] _jumps = new JumpPatch[64];
		private int _jumpCount = 0;

		private readonly Dictionary<GssValue, int> _constantsPoolMap = new(new GssValueComparer());
		private readonly List<GssValue> _constantsPool = new();

		private readonly Dictionary<object, int> _metadataMap = new();
		private readonly List<object> _metadataPool = new();

		private RegisterAllocator _allocator = null!;

		public ExecutableGraph Emit(BoundBlockStatement root, RegisterAllocator allocator)
		{
			_allocator = allocator;

			for (int i = 0; i < root.Statements.Count; i++)
			{
				EmitStatement(root.Statements[i]);
			}

			EmitInstruction(new Instruction(OpCode.Halt));

			for (int i = 0; i < _jumpCount; i++)
			{
				ref JumpPatch patch = ref _jumps[i];
				int targetOffset = _labelOffsets[patch.Target];

				if (patch.IsBranch)
				{
					int trueJump = patch.JumpIfTrue ? targetOffset : patch.InstIndex + 1;
					int falseJump = patch.JumpIfTrue ? patch.InstIndex + 1 : targetOffset;
					_instructions[patch.InstIndex] = new Instruction(patch.JumpIfTrue ? OpCode.BranchTrue : OpCode.BranchFalse, trueJump, falseJump);
				}
				else
				{
					_instructions[patch.InstIndex] = new Instruction(OpCode.Jump, targetOffset);
				}
			}

			return new ExecutableGraph(_instructions.ToArray(), _constantsPool.ToArray(), _metadataPool.ToArray(), _allocator.GetTotalRequiredRegisters());
		}

		private void EmitInstruction(Instruction inst) => _instructions.Add(inst);
		private void MarkLabel(BoundLabel label) => _labelOffsets[label] = _instructions.Count;

		private void RegisterJumpPatch(BoundLabel target, bool isBranch, bool jumpIfTrue)
		{
			if (_jumpCount >= _jumps.Length) Array.Resize(ref _jumps, _jumps.Length * 2);
			_jumps[_jumpCount++] = new JumpPatch { InstIndex = _instructions.Count, Target = target, IsBranch = isBranch, JumpIfTrue = jumpIfTrue };
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
				case BoundNodeKind.WaitStatement:
					EmitExpression(((BoundWaitStatement)node).Duration);
					EmitInstruction(new Instruction(OpCode.Wait));
					break;
				case BoundNodeKind.LabelStatement:
					MarkLabel(((BoundLabelStatement)node).Label);
					break;
				case BoundNodeKind.GotoStatement:
					RegisterJumpPatch(((BoundGotoStatement)node).Label, false, false);
					break;
				case BoundNodeKind.ConditionalGotoStatement:
					var cgs = (BoundConditionalGotoStatement)node;
					EmitExpression(cgs.Condition);
					RegisterJumpPatch(cgs.Label, true, cgs.JumpIfTrue);
					break;
				case BoundNodeKind.VariableDeclaration:
					var decl = (BoundVariableDeclaration)node;
					EmitExpression(decl.Initializer);
					EmitInstruction(new Instruction(OpCode.StoreReg, decl.Variable.RegisterIndex));
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
						int reg = _allocator.Allocate();
						EmitInstruction(new Instruction(OpCode.StoreReg, reg));
						args[i] = reg;
					}

					for (int i = 0; i < args.Length; i++)
					{
						EmitInstruction(new Instruction(OpCode.PushArg, args[i], i));
						_allocator.Free(args[i]);
					}

					EmitInstruction(new Instruction(OpCode.CallMethod, GetOrAddMetadata(call.Method), args.Length));
					break;

				case BoundNodeKind.IndexExpression:
					var idx = (BoundIndexExpression)node;
					EmitExpression(idx.Collection);
					int colReg = _allocator.Allocate();
					EmitInstruction(new Instruction(OpCode.StoreReg, colReg));
					EmitExpression(idx.Index);
					EmitInstruction(new Instruction(OpCode.LoadElement_GssArray, colReg));
					_allocator.Free(colReg);
					break;

				case BoundNodeKind.IndexAssignmentExpression:
					var idxAss = (BoundIndexAssignmentExpression)node;
					EmitExpression(idxAss.Collection);
					int cr = _allocator.Allocate();
					EmitInstruction(new Instruction(OpCode.StoreReg, cr));
					EmitExpression(idxAss.Index);
					int ir = _allocator.Allocate();
					EmitInstruction(new Instruction(OpCode.StoreReg, ir));
					EmitExpression(idxAss.Expression);
					EmitInstruction(new Instruction(OpCode.StoreElement_GssArray, cr, ir));
					_allocator.Free(ir);
					_allocator.Free(cr);
					break;

				case BoundNodeKind.ConversionExpression:
					var conv = (BoundConversionExpression)node;
					EmitExpression(conv.Expression);

					if (conv.Type == TypeSymbol.Float && conv.Expression.Type == TypeSymbol.Int)
						EmitInstruction(new Instruction(OpCode.CastIntToFloat));
					else if (conv.Type == TypeSymbol.Int && conv.Expression.Type == TypeSymbol.Float)
						EmitInstruction(new Instruction(OpCode.CastFloatToInt));
					else if (conv.Type == TypeSymbol.String && conv.Expression.Type != TypeSymbol.String)
						EmitInstruction(new Instruction(OpCode.CastAnyToString));
					break;

				case BoundNodeKind.UnaryExpression:
					var unary = (BoundUnaryExpression)node;
					EmitExpression(unary.Operand);
					OpCode unOp = unary.Op.Kind switch
					{
						BoundUnaryOperatorKind.Negation => unary.Type == TypeSymbol.Int ? OpCode.NegateInt : OpCode.NegateFloat,
						BoundUnaryOperatorKind.LogicalNegation => OpCode.NotBool,
						BoundUnaryOperatorKind.BitwiseNegation => OpCode.BitNotInt,
						BoundUnaryOperatorKind.Identity => OpCode.Nop,
						_ => throw new InvalidOperationException($"Unary operator {unary.Op.Kind} not mapped.")
					};
					if (unOp != OpCode.Nop)
						EmitInstruction(new Instruction(unOp));
					break;

				case BoundNodeKind.BinaryExpression:
					var bin = (BoundBinaryExpression)node;

					if (bin.Op.Kind == BoundBinaryOperatorKind.LogicalAnd || bin.Op.Kind == BoundBinaryOperatorKind.LogicalOr)
					{
						EmitExpression(bin.Left);
						int patchIdx = _instructions.Count;
						EmitInstruction(new Instruction(OpCode.Nop));
						EmitExpression(bin.Right);
						_instructions[patchIdx] = new Instruction(bin.Op.Kind == BoundBinaryOperatorKind.LogicalAnd ? OpCode.BranchFalse : OpCode.BranchTrue, _instructions.Count, patchIdx + 1);
						return;
					}

					EmitExpression(bin.Right);
					int rReg = _allocator.Allocate();
					EmitInstruction(new Instruction(OpCode.StoreReg, rReg));
					EmitExpression(bin.Left);

					OpCode op = GetBinaryOpCode(bin.Op.Kind, bin.Left.Type);
					EmitInstruction(new Instruction(op, rReg));
					_allocator.Free(rReg);
					break;

				case BoundNodeKind.TernaryExpression:
					var ternary = (BoundTernaryExpression)node;
					var falseLabel = new BoundLabel("tern_false");
					var endLabel = new BoundLabel("tern_end");

					EmitExpression(ternary.Condition);
					RegisterJumpPatch(falseLabel, true, false);

					EmitExpression(ternary.TrueExpression);
					RegisterJumpPatch(endLabel, false, false);

					MarkLabel(falseLabel);
					EmitExpression(ternary.FalseExpression);

					MarkLabel(endLabel);
					break;

				case BoundNodeKind.ApiPropertyExpression:
					EmitInstruction(new Instruction(OpCode.LoadProp, GetOrAddMetadata(((BoundApiPropertyExpression)node).Property)));
					break;
			}
		}

		private OpCode GetBinaryOpCode(BoundBinaryOperatorKind kind, TypeSymbol leftType)
		{
			return kind switch
			{
				BoundBinaryOperatorKind.Addition => leftType == TypeSymbol.String ? OpCode.AddString : (leftType == TypeSymbol.Int ? OpCode.AddInt : OpCode.AddFloat),
				BoundBinaryOperatorKind.Subtraction => leftType == TypeSymbol.Int ? OpCode.SubInt : OpCode.SubFloat,
				BoundBinaryOperatorKind.Multiplication => leftType == TypeSymbol.Int ? OpCode.MulInt : OpCode.MulFloat,
				BoundBinaryOperatorKind.Division => leftType == TypeSymbol.Int ? OpCode.DivInt : OpCode.DivFloat,
				BoundBinaryOperatorKind.Modulo => leftType == TypeSymbol.Int ? OpCode.ModInt : OpCode.ModFloat,

				BoundBinaryOperatorKind.Equals =>
					leftType == TypeSymbol.Int ? OpCode.EqInt :
					leftType == TypeSymbol.Float ? OpCode.EqFloat :
					leftType == TypeSymbol.Bool ? OpCode.EqBool : OpCode.EqString,

				BoundBinaryOperatorKind.NotEquals =>
					leftType == TypeSymbol.Int ? OpCode.NeqInt :
					leftType == TypeSymbol.Float ? OpCode.NeqFloat :
					leftType == TypeSymbol.Bool ? OpCode.NeqBool : OpCode.NeqString,

				BoundBinaryOperatorKind.Less => leftType == TypeSymbol.Int ? OpCode.LtInt : OpCode.LtFloat,
				BoundBinaryOperatorKind.LessOrEquals => leftType == TypeSymbol.Int ? OpCode.LteInt : OpCode.LteFloat,
				BoundBinaryOperatorKind.Greater => leftType == TypeSymbol.Int ? OpCode.GtInt : OpCode.GtFloat,
				BoundBinaryOperatorKind.GreaterOrEquals => leftType == TypeSymbol.Int ? OpCode.GteInt : OpCode.GteFloat,

				_ => throw new InvalidOperationException($"Binary operator '{kind}' is not mapped to an OpCode for type '{leftType.Name}'.")
			};
		}
	}
}