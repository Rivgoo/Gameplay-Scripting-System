using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Helpers;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	#region Int Conversions
	public sealed class CastIntToFloatInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastIntToDoubleInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastIntToLongInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastIntToStringInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.AsInt.ToString());
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Float Conversions
	public sealed class CastFloatToIntInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt((int)ctx.Accumulator.AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastFloatToDoubleInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastFloatToLongInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong((long)ctx.Accumulator.AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastFloatToStringInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.AsFloat.ToString());
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Double Conversions
	public sealed class CastDoubleToIntInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt((int)ctx.Accumulator.AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastDoubleToFloatInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat((float)ctx.Accumulator.AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastDoubleToLongInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong((long)ctx.Accumulator.AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastDoubleToStringInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.AsDouble.ToString());
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Long Conversions
	public sealed class CastLongToIntInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt((int)ctx.Accumulator.AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastLongToFloatInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastLongToDoubleInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastLongToStringInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.AsLong.ToString());
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Boolean Conversions
	public sealed class CastBoolToIntInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsBool ? 1 : 0);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class CastBoolToStringInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.AsBool ? "True" : "False");
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Dynamic/Object Conversions
	public sealed class CastAnyToStringInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromObject(ctx.Accumulator.GetAsString());
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}