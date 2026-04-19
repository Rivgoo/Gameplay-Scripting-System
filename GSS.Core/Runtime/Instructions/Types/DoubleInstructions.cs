using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	#region Arithmetic
	public sealed class AddDoubleInstruction : IInstruction
	{
		private readonly int _reg; public AddDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsDouble + ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class SubDoubleInstruction : IInstruction
	{
		private readonly int _reg; public SubDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsDouble - ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MulDoubleInstruction : IInstruction
	{
		private readonly int _reg; public MulDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsDouble * ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class DivDoubleInstruction : IInstruction
	{
		private readonly int _reg; public DivDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			double right = ctx.GetRegister(_reg).AsDouble;
			if (right == 0d) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsDouble / right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ModDoubleInstruction : IInstruction
	{
		private readonly int _reg; public ModDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			double right = ctx.GetRegister(_reg).AsDouble;
			if (right == 0d) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromDouble(ctx.Accumulator.AsDouble % right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Comparison
	public sealed class EqDoubleInstruction : IInstruction
	{
		private readonly int _reg; public EqDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsDouble == ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class NeqDoubleInstruction : IInstruction
	{
		private readonly int _reg; public NeqDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsDouble != ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GtDoubleInstruction : IInstruction
	{
		private readonly int _reg; public GtDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsDouble > ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LtDoubleInstruction : IInstruction
	{
		private readonly int _reg; public LtDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsDouble < ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GteDoubleInstruction : IInstruction
	{
		private readonly int _reg; public GteDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsDouble >= ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LteDoubleInstruction : IInstruction
	{
		private readonly int _reg; public LteDoubleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsDouble <= ctx.GetRegister(_reg).AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Unary
	public sealed class NegateDoubleInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromDouble(-ctx.Accumulator.AsDouble);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}