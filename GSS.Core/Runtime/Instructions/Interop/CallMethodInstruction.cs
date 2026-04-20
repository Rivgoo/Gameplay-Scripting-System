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
			for (int i = 0; i < _argumentRegisters.Length; i++)
			{
				var value = context.GetRegister(_argumentRegisters[i]);
				context.SetArg(i, value);
			}

			var argSpan = context.GetArgBufferSpan(_argumentRegisters.Length);

			context.Accumulator = _method.Invoke(context.TargetInstance, argSpan);

			context.InstructionPointer++;

			return ExecutionState.Success;
		}
	}
}