using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Interop
{
	public sealed class LoadPropertyInstruction : IInstruction
	{
		private readonly IPropertyMetadata _property;

		public LoadPropertyInstruction(IPropertyMetadata property)
		{
			_property = property;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			context.Accumulator = _property.GetValue(context.TargetInstance);
			context.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}