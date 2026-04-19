using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Core
{
	public sealed class JumpInstruction : IInstruction
	{
		private readonly int _targetIndex;

		public JumpInstruction(int targetIndex)
		{
			_targetIndex = targetIndex;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			context.InstructionPointer = _targetIndex;
			return ExecutionState.Success;
		}
	}
}