using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Memory
{
	public sealed class LoadConstantInstruction : IInstruction
	{
		private readonly GssValue _constant;

		public LoadConstantInstruction(GssValue constant)
		{
			_constant = constant;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			context.Accumulator = _constant;
			context.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}