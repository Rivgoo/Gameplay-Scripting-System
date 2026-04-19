using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Async
{
	public sealed class WaitInstruction : IInstruction
	{
		private readonly float _duration;

		public WaitInstruction(float duration)
		{
			_duration = duration;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			if (context.StateTimer >= _duration)
			{
				context.StateTimer = 0f;
				context.InstructionPointer++;
				return ExecutionState.Success;
			}

			return ExecutionState.Running;
		}
	}
}