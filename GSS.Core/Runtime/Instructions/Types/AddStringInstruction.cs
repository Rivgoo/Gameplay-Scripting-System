using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Helpers;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	public sealed class AddStringInstruction : IInstruction
	{
		private readonly int _reg; 

		public AddStringInstruction(int reg) => _reg = reg;
        
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			string left = ctx.Accumulator.GetAsString();
			string right = ctx.GetRegister(_reg).GetAsString();
            
			ctx.Accumulator = GssValue.FromObject(left + right);
			ctx.InstructionPointer++; 
            
			return ExecutionState.Success;
		}
	}
}