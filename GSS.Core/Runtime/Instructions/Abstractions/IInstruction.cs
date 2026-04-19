using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Abstractions
{
	public interface IInstruction
	{
		ExecutionState Execute(RuntimeExecutionContext context);
	}
}
