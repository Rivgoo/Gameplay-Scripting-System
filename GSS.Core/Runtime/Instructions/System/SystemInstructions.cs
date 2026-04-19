using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.System
{
	public sealed class NoOpInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}

	public sealed class HaltInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			return ExecutionState.Halted;
		}
	}

	public sealed class AbortInstruction : IInstruction
	{
		private readonly string _reason;

		public AbortInstruction(string reason)
		{
			_reason = reason;
		}

		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			throw new GssRuntimeException($"Script explicitly aborted: {_reason}");
		}
	}

	public sealed class RuntimeLogInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			Console.WriteLine($"[GSS RUNTIME LOG] ACC: {ctx.Accumulator.Type} | Value: {ctx.Accumulator.ToBoxedValue()}");
			ctx.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}