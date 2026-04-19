using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Memory
{
	public sealed class LoadInstruction : IInstruction
	{
		private readonly int _registerIndex;

		public LoadInstruction(int registerIndex)
		{
			_registerIndex = registerIndex;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			context.Accumulator = context.GetRegister(_registerIndex);
			context.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}