using System.Collections;
using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.Compiler.Emission;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Events.Abstractions;
using GSS.Core.Runtime.Exceptions;
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
					var state = RunContext(context);
					if (state == ExecutionState.Failure)
					{
						var ex = new GssRuntimeException(context.ErrorMessage ?? "Unknown execution failure.");
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

		private ExecutionState RunContext(RuntimeExecutionContext ctx)
		{
			var instructions = ctx.Graph.Instructions;
			var constants = ctx.Graph.ConstantsPool;
			var metadata = ctx.Graph.MetadataPool;
			int maxSteps = MaxStepsPerTick;
			int steps = 0;

			ref int ip = ref ctx.InstructionPointer;

			while (ip >= 0 && ip < instructions.Length)
			{
				if (steps++ >= maxSteps) return ExecutionState.Running;

				var inst = instructions[ip];

				switch (inst.Code)
				{
					case OpCode.Nop: ip++; break;
					case OpCode.Jump: ip = inst.Op1; break;
					case OpCode.BranchTrue: ip = ctx.Accumulator.AsBool ? inst.Op1 : inst.Op2; break;
					case OpCode.BranchFalse: ip = !ctx.Accumulator.AsBool ? inst.Op1 : inst.Op2; break;
					case OpCode.LoadConst: ctx.Accumulator = constants[inst.Op1]; ip++; break;
					case OpCode.LoadReg: ctx.Accumulator = ctx.GetRegister(inst.Op1); ip++; break;
					case OpCode.StoreReg: ctx.SetRegister(inst.Op1, ctx.Accumulator); ip++; break;
					case OpCode.PushArg: ctx.SetArg(inst.Op2, ctx.GetRegister(inst.Op1)); ip++; break;

					case OpCode.CallMethod:
						var method = (IMethodMetadata)metadata[inst.Op1];
						ctx.Accumulator = method.Invoke(ctx.TargetInstance, ctx.GetArgBufferSpan(inst.Op2));
						ip++;
						break;

					case OpCode.LoadProp:
						var propGet = (IPropertyMetadata)metadata[inst.Op1];
						ctx.Accumulator = propGet.GetValue(ctx.TargetInstance);
						ip++;
						break;

					case OpCode.StoreProp:
						var propSet = (IPropertyMetadata)metadata[inst.Op1];
						propSet.SetValue(ctx.TargetInstance, ctx.Accumulator);
						ip++;
						break;

					case OpCode.AddInt: ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt + ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.SubInt: ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt - ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.MulInt: ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt * ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.DivInt:
						int rDiv = ctx.GetRegister(inst.Op1).AsInt;
						if (rDiv == 0) { ctx.SetError("Divide by zero."); return ExecutionState.Failure; }
						ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt / rDiv); ip++; break;
					case OpCode.ModInt:
						int rMod = ctx.GetRegister(inst.Op1).AsInt;
						if (rMod == 0) { ctx.SetError("Modulo by zero."); return ExecutionState.Failure; }
						ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt % rMod); ip++; break;

					case OpCode.AddFloat: ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat + ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.SubFloat: ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat - ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.MulFloat: ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat * ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.DivFloat:
						float rDivF = ctx.GetRegister(inst.Op1).AsFloat;
						if (rDivF == 0f) { ctx.SetError("Divide by zero."); return ExecutionState.Failure; }
						ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat / rDivF); ip++; break;

					case OpCode.AddString:
						string left = ctx.Accumulator.GetAsString();
						string right = ctx.GetRegister(inst.Op1).GetAsString();
						ctx.Accumulator = GssValue.FromObject(left + right);
						ip++; break;

					case OpCode.EqInt: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt == ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.NeqInt: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt != ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.LtInt: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt < ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.GtInt: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt > ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.LteInt: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt <= ctx.GetRegister(inst.Op1).AsInt); ip++; break;
					case OpCode.GteInt: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt >= ctx.GetRegister(inst.Op1).AsInt); ip++; break;

					case OpCode.EqFloat: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat == ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.NeqFloat: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat != ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.LtFloat: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat < ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.GtFloat: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat > ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.LteFloat: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat <= ctx.GetRegister(inst.Op1).AsFloat); ip++; break;
					case OpCode.GteFloat: ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat >= ctx.GetRegister(inst.Op1).AsFloat); ip++; break;

					case OpCode.NotBool: ctx.Accumulator = GssValue.FromBool(!ctx.Accumulator.AsBool); ip++; break;
					case OpCode.NegateInt: ctx.Accumulator = GssValue.FromInt(-ctx.Accumulator.AsInt); ip++; break;
					case OpCode.NegateFloat: ctx.Accumulator = GssValue.FromFloat(-ctx.Accumulator.AsFloat); ip++; break;
					case OpCode.BitNotInt: ctx.Accumulator = GssValue.FromInt(~ctx.Accumulator.AsInt); ip++; break;
					case OpCode.BitNotLong: ctx.Accumulator = GssValue.FromLong(~ctx.Accumulator.AsLong); ip++; break;

					case OpCode.CastIntToFloat: ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsInt); ip++; break;
					case OpCode.CastFloatToInt: ctx.Accumulator = GssValue.FromInt((int)ctx.Accumulator.AsFloat); ip++; break;
					case OpCode.CastAnyToString: ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.GetAsString()); ip++; break;

					case OpCode.LoadElement:
						if (!TryLoadElement(ctx.GetRegister(inst.Op1).AsObject, ctx.Accumulator.AsInt, out var valLoad, out string errLoad))
						{
							ctx.SetError(errLoad); return ExecutionState.Failure;
						}
						ctx.Accumulator = valLoad;
						ip++; break;

					case OpCode.StoreElement:
						if (!TryStoreElement(ctx.GetRegister(inst.Op1).AsObject, ctx.GetRegister(inst.Op2).AsInt, ctx.Accumulator, out string errStore))
						{
							ctx.SetError(errStore); return ExecutionState.Failure;
						}
						ip++; break;

					case OpCode.Wait:
						if (ctx.StateTimer >= ctx.Accumulator.AsFloat)
						{
							ctx.StateTimer = 0f; ip++;
						}
						else
						{
							return ExecutionState.Running;
						}
						break;

					case OpCode.Halt:
						return ExecutionState.Success;

					default:
						ctx.SetError($"Unsupported OpCode execution: {inst.Code}");
						return ExecutionState.Failure;
				}
			}

			ctx.SetError("Instruction pointer went out of bounds without Halt.");
			return ExecutionState.Failure;
		}

		private bool TryLoadElement(object? col, int index, out GssValue result, out string error)
		{
			result = GssValue.Null; error = string.Empty;
			if (col is GssValue[] array)
			{
				if (index < 0 || index >= array.Length) { error = "Index out of bounds."; return false; }
				result = array[index]; return true;
			}
			if (col is List<GssValue> list)
			{
				if (index < 0 || index >= list.Count) { error = "Index out of bounds."; return false; }
				result = list[index]; return true;
			}
			if (col is IList ilist)
			{
				if (index < 0 || index >= ilist.Count) { error = "Index out of bounds."; return false; }
				result = GssValuePacker.Pack(ilist[index]); return true;
			}
			error = "Target is not a valid collection.";
			return false;
		}

		private bool TryStoreElement(object? col, int index, GssValue value, out string error)
		{
			error = string.Empty;
			if (col is GssValue[] array)
			{
				if (index < 0 || index >= array.Length) { error = "Index out of bounds."; return false; }
				array[index] = value; return true;
			}
			if (col is List<GssValue> list)
			{
				if (index < 0 || index >= list.Count) { error = "Index out of bounds."; return false; }
				list[index] = value; return true;
			}
			if (col is IList ilist)
			{
				if (index < 0 || index >= ilist.Count) { error = "Index out of bounds."; return false; }
				ilist[index] = value.ToBoxedValue(); return true;
			}
			error = "Target is not a valid modifiable collection.";
			return false;
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