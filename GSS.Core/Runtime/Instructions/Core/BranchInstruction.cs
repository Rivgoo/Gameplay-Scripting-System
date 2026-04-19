using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Core
{
	public sealed class BranchInstruction : IInstruction
	{
		private readonly int _trueJumpIndex;
		private readonly int _falseJumpIndex;

		public BranchInstruction(int trueJumpIndex, int falseJumpIndex)
		{
			_trueJumpIndex = trueJumpIndex;
			_falseJumpIndex = falseJumpIndex;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			bool condition = context.Accumulator.Unbox<bool>();
			context.InstructionPointer = condition ? _trueJumpIndex : _falseJumpIndex;

			return ExecutionState.Success;
		}
	}
}