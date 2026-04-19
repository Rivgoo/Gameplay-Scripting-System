using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Interop
{
	public sealed class CallMethodInstruction : IInstruction
	{
		private readonly IMethodMetadata _method;
		private readonly int[] _argumentRegisters;

		public CallMethodInstruction(IMethodMetadata method, int[] argumentRegisters)
		{
			_method = method;
			_argumentRegisters = argumentRegisters;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			var buffer = context.GetArgBuffer(_argumentRegisters.Length);

			for (int i = 0; i < _argumentRegisters.Length; i++)
			{
				buffer[i] = context.GetRegister(_argumentRegisters[i]);
			}

			context.Accumulator = _method.Invoke(context.TargetInstance, buffer);
			context.InstructionPointer++;

			return ExecutionState.Success;
		}
	}
}