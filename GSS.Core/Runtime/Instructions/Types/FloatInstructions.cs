using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	#region Arithmetic
	public sealed class AddFloatInstruction : IInstruction
	{
		private readonly int _reg; public AddFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat + ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class SubFloatInstruction : IInstruction
	{
		private readonly int _reg; public SubFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat - ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MulFloatInstruction : IInstruction
	{
		private readonly int _reg; public MulFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat * ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class DivFloatInstruction : IInstruction
	{
		private readonly int _reg; public DivFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float right = ctx.GetRegister(_reg).AsFloat;
			if (right == 0f) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat / right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ModFloatInstruction : IInstruction
	{
		private readonly int _reg; public ModFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float right = ctx.GetRegister(_reg).AsFloat;
			if (right == 0f) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromFloat(ctx.Accumulator.AsFloat % right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Comparison
	public sealed class EqFloatInstruction : IInstruction
	{
		private readonly int _reg; public EqFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat == ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class NeqFloatInstruction : IInstruction
	{
		private readonly int _reg; public NeqFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat != ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GtFloatInstruction : IInstruction
	{
		private readonly int _reg; public GtFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat > ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LtFloatInstruction : IInstruction
	{
		private readonly int _reg; public LtFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat < ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GteFloatInstruction : IInstruction
	{
		private readonly int _reg; public GteFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat >= ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LteFloatInstruction : IInstruction
	{
		private readonly int _reg; public LteFloatInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsFloat <= ctx.GetRegister(_reg).AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Unary
	public sealed class NegateFloatInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(-ctx.Accumulator.AsFloat);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}