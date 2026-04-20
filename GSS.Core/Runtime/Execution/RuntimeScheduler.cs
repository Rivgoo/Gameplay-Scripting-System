using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.Compiler.Emission;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Events.Abstractions;
using GSS.Core.Runtime.Helpers;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Execution
{
	public sealed class RuntimeScheduler
	{
		private readonly ExecutionContextPool _pool;
		private readonly List<RuntimeExecutionContext> _activeContexts;

		public IGlobalDependencyResolver? GlobalResolver { get; set; }
		public int MaxStepsPerTick { get; set; } = 10000;
		public Action<Exception, RuntimeExecutionContext>? OnScriptFaulted { get; set; }

		public int ActiveContextsCount => _activeContexts.Count;
		public bool IsRunning => _activeContexts.Count > 0;

		public RuntimeScheduler(ExecutionContextPool pool, int initialCapacity = 256)
		{
			_pool = pool;
			_activeContexts = new List<RuntimeExecutionContext>(initialCapacity);
		}

		public void Enqueue(object instance, ExecutableGraph graph, IEventPayload? payload = null)
		{
			var context = _pool.Rent();
			context.Initialize(instance, graph, GlobalResolver);
			payload?.WriteToContext(context);
			_activeContexts.Add(context);
		}

		public void Tick(float deltaTime)
		{
			for (int i = _activeContexts.Count - 1; i >= 0; i--)
			{
				var context = _activeContexts[i];
				context.StateTimer += deltaTime;

				try
				{
					var state = ExecuteLoop(context);

					if (state == ExecutionState.Failure)
					{
						var ex = new Exception(context.ErrorMessage ?? "Unknown VM failure.");
						OnScriptFaulted?.Invoke(ex, context);
						RemoveContextAt(i);
					}
					else if (state != ExecutionState.Running)
					{
						RemoveContextAt(i);
					}
				}
				catch (Exception ex)
				{
					OnScriptFaulted?.Invoke(ex, context);
					RemoveContextAt(i);
				}
			}
		}

		private ExecutionState ExecuteLoop(RuntimeExecutionContext ctx)
		{
			ReadOnlySpan<Instruction> instructions = ctx.Graph.Instructions;
			ReadOnlySpan<GssValue> constants = ctx.Graph.ConstantsPool;
			ReadOnlySpan<object> metadata = ctx.Graph.MetadataPool;

			int maxSteps = MaxStepsPerTick;
			int steps = 0;

			ref int ip = ref ctx.InstructionPointer;
			ref GssValue acc = ref ctx.Accumulator;

			while (ip >= 0 && ip < instructions.Length)
			{
				if (steps++ >= maxSteps) return ExecutionState.Running;

				ref readonly Instruction inst = ref instructions[ip];

				switch (inst.Code)
				{
					case OpCode.Nop: ip++; break;
					case OpCode.Halt: return ExecutionState.Success;
					case OpCode.Abort: ctx.SetError("Script explicitly aborted."); return ExecutionState.Failure;
					case OpCode.Wait:
						if (ctx.StateTimer >= acc.AsFloat) { ctx.StateTimer = 0f; ip++; }
						else return ExecutionState.Running;
						break;
					case OpCode.Jump: ip = inst.Op1; break;
					case OpCode.BranchTrue: ip = acc.AsBool ? inst.Op1 : inst.Op2; break;
					case OpCode.BranchFalse: ip = !acc.AsBool ? inst.Op1 : inst.Op2; break;
					case OpCode.LoadConst: acc = constants[inst.Op1]; ip++; break;
					case OpCode.LoadReg: acc = ctx.GetRegisterRef(inst.Op1); ip++; break;
					case OpCode.StoreReg: ctx.GetRegisterRef(inst.Op1) = acc; ip++; break;
					case OpCode.PushArg: ctx.SetArg(inst.Op2, ctx.GetRegisterRef(inst.Op1)); ip++; break;
					case OpCode.CallMethod:
						var method = (IMethodMetadata)metadata[inst.Op1];
						acc = method.Invoke(ctx.TargetInstance, ctx.GetArgBufferSpan(inst.Op2)); ip++; break;
					case OpCode.LoadProp:
						var propGet = (IPropertyMetadata)metadata[inst.Op1];
						acc = propGet.GetValue(ctx.TargetInstance); ip++; break;
					case OpCode.StoreProp:
						var propSet = (IPropertyMetadata)metadata[inst.Op1];
						propSet.SetValue(ctx.TargetInstance, acc); ip++; break;

					// Arithmetic
					case OpCode.AddInt: acc = GssValue.FromInt(acc.AsInt + ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.SubInt: acc = GssValue.FromInt(acc.AsInt - ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.MulInt: acc = GssValue.FromInt(acc.AsInt * ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.DivInt:
						int divI = ctx.GetRegisterRef(inst.Op1).AsInt;
						if (divI == 0) { ctx.SetError("Divide by zero."); return ExecutionState.Failure; }
						acc = GssValue.FromInt(acc.AsInt / divI); ip++; break;
					case OpCode.ModInt:
						int modI = ctx.GetRegisterRef(inst.Op1).AsInt;
						if (modI == 0) { ctx.SetError("Modulo by zero."); return ExecutionState.Failure; }
						acc = GssValue.FromInt(acc.AsInt % modI); ip++; break;

					case OpCode.AddFloat: acc = GssValue.FromFloat(acc.AsFloat + ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.SubFloat: acc = GssValue.FromFloat(acc.AsFloat - ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.MulFloat: acc = GssValue.FromFloat(acc.AsFloat * ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.DivFloat:
						float divF = ctx.GetRegisterRef(inst.Op1).AsFloat;
						if (divF == 0f) { ctx.SetError("Divide by zero."); return ExecutionState.Failure; }
						acc = GssValue.FromFloat(acc.AsFloat / divF); ip++; break;
					case OpCode.ModFloat:
						float modF = ctx.GetRegisterRef(inst.Op1).AsFloat;
						if (modF == 0f) { ctx.SetError("Modulo by zero."); return ExecutionState.Failure; }
						acc = GssValue.FromFloat(acc.AsFloat % modF); ip++; break;

					// String
					case OpCode.AddString:
						acc = GssValue.FromObject(acc.GetAsString() + ctx.GetRegisterRef(inst.Op1).GetAsString()); ip++; break;
					case OpCode.EqString:
						acc = GssValue.FromBool(acc.GetAsString() == ctx.GetRegisterRef(inst.Op1).GetAsString()); ip++; break;
					case OpCode.NeqString:
						acc = GssValue.FromBool(acc.GetAsString() != ctx.GetRegisterRef(inst.Op1).GetAsString()); ip++; break;

					// Comparisons
					case OpCode.EqInt: acc = GssValue.FromBool(acc.AsInt == ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.NeqInt: acc = GssValue.FromBool(acc.AsInt != ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.LtInt: acc = GssValue.FromBool(acc.AsInt < ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.GtInt: acc = GssValue.FromBool(acc.AsInt > ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.LteInt: acc = GssValue.FromBool(acc.AsInt <= ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.GteInt: acc = GssValue.FromBool(acc.AsInt >= ctx.GetRegisterRef(inst.Op1).AsInt); ip++; break;
					case OpCode.EqFloat: acc = GssValue.FromBool(acc.AsFloat == ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.NeqFloat: acc = GssValue.FromBool(acc.AsFloat != ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.LtFloat: acc = GssValue.FromBool(acc.AsFloat < ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.GtFloat: acc = GssValue.FromBool(acc.AsFloat > ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.LteFloat: acc = GssValue.FromBool(acc.AsFloat <= ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;
					case OpCode.GteFloat: acc = GssValue.FromBool(acc.AsFloat >= ctx.GetRegisterRef(inst.Op1).AsFloat); ip++; break;

					// Boolean
					case OpCode.EqBool: acc = GssValue.FromBool(acc.AsBool == ctx.GetRegisterRef(inst.Op1).AsBool); ip++; break;
					case OpCode.NeqBool: acc = GssValue.FromBool(acc.AsBool != ctx.GetRegisterRef(inst.Op1).AsBool); ip++; break;
					case OpCode.AndBool: acc = GssValue.FromBool(acc.AsBool & ctx.GetRegisterRef(inst.Op1).AsBool); ip++; break;
					case OpCode.OrBool: acc = GssValue.FromBool(acc.AsBool | ctx.GetRegisterRef(inst.Op1).AsBool); ip++; break;
					case OpCode.XorBool: acc = GssValue.FromBool(acc.AsBool ^ ctx.GetRegisterRef(inst.Op1).AsBool); ip++; break;
					case OpCode.NotBool: acc = GssValue.FromBool(!acc.AsBool); ip++; break;

					// Unary / Casts
					case OpCode.NegateInt: acc = GssValue.FromInt(-acc.AsInt); ip++; break;
					case OpCode.NegateFloat: acc = GssValue.FromFloat(-acc.AsFloat); ip++; break;
					case OpCode.BitNotInt: acc = GssValue.FromInt(~acc.AsInt); ip++; break;
					case OpCode.CastIntToFloat: acc = GssValue.FromFloat(acc.AsInt); ip++; break;
					case OpCode.CastFloatToInt: acc = GssValue.FromInt((int)acc.AsFloat); ip++; break;
					case OpCode.CastAnyToString: acc = GssValue.FromObject(acc.GetAsString()); ip++; break;

					// Collections
					case OpCode.CreateArray: acc = GssValue.FromObject(new GssValue[acc.AsInt]); ip++; break;
					case OpCode.CreateList: acc = GssValue.FromObject(new List<GssValue>()); ip++; break;
					case OpCode.GetArrayLength:
						var arrLen = (GssValue[])ctx.GetRegisterRef(inst.Op1).AsObject!;
						acc = GssValue.FromInt(arrLen.Length); ip++; break;
					case OpCode.LoadElement_GssArray:
						var gssArrLoad = (GssValue[])ctx.GetRegisterRef(inst.Op1).AsObject!;
						int idxGssArr = acc.AsInt;
						if (idxGssArr < 0 || idxGssArr >= gssArrLoad.Length) { ctx.SetError("Index out of bounds."); return ExecutionState.Failure; }
						acc = gssArrLoad[idxGssArr]; ip++; break;
					case OpCode.StoreElement_GssArray:
						var gssArrStore = (GssValue[])ctx.GetRegisterRef(inst.Op1).AsObject!;
						int idxGssStore = ctx.GetRegisterRef(inst.Op2).AsInt;
						if (idxGssStore < 0 || idxGssStore >= gssArrStore.Length) { ctx.SetError("Index out of bounds."); return ExecutionState.Failure; }
						gssArrStore[idxGssStore] = acc; ip++; break;

					default:
						ctx.SetError($"Unsupported OpCode execution: {inst.Code}");
						return ExecutionState.Failure;
				}
			}

			ctx.SetError("Instruction pointer out of bounds.");
			return ExecutionState.Failure;
		}

		private void RemoveContextAt(int index)
		{
			var context = _activeContexts[index];
			_pool.Return(context);

			int lastIndex = _activeContexts.Count - 1;
			if (index != lastIndex) _activeContexts[index] = _activeContexts[lastIndex];
			_activeContexts.RemoveAt(lastIndex);
		}
	}
}