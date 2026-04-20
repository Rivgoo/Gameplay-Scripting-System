using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Interop
{
	public sealed class StorePropertyInstruction : IInstruction
	{
		private readonly IPropertyMetadata _property;

		public StorePropertyInstruction(IPropertyMetadata property)
		{
			_property = property;
		}

		public ExecutionState Execute(RuntimeExecutionContext context)
		{
			_property.SetValue(context.TargetInstance, context.Accumulator);
			context.InstructionPointer++;
			return ExecutionState.Success;
		}
	}
}