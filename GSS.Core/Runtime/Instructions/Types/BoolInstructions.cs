using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	#region Bitwise / Logical
	public sealed class AndBoolInstruction : IInstruction
	{
		private readonly int _reg; public AndBoolInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsBool & ctx.GetRegister(_reg).AsBool);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class OrBoolInstruction : IInstruction
	{
		private readonly int _reg; public OrBoolInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsBool | ctx.GetRegister(_reg).AsBool);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class XorBoolInstruction : IInstruction
	{
		private readonly int _reg; public XorBoolInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsBool ^ ctx.GetRegister(_reg).AsBool);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Comparison
	public sealed class EqBoolInstruction : IInstruction
	{
		private readonly int _reg; public EqBoolInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsBool == ctx.GetRegister(_reg).AsBool);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class NeqBoolInstruction : IInstruction
	{
		private readonly int _reg; public NeqBoolInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsBool != ctx.GetRegister(_reg).AsBool);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Unary
	public sealed class NotBoolInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(!ctx.Accumulator.AsBool);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}