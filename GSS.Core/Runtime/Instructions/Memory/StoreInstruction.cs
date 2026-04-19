using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Memory
{
	public sealed class StoreInstruction : IInstruction
	{
		private readonly int _registerIndex;

		public StoreInstruction(int registerIndex)
		{
			_registerIndex = registerIndex;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			context.SetRegister(_registerIndex, context.Accumulator);
			context.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}