using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	#region Arithmetic
	public sealed class AddLongInstruction : IInstruction
	{
		private readonly int _reg; public AddLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong + ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class SubLongInstruction : IInstruction
	{
		private readonly int _reg; public SubLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong - ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MulLongInstruction : IInstruction
	{
		private readonly int _reg; public MulLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong * ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class DivLongInstruction : IInstruction
	{
		private readonly int _reg; public DivLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			long right = ctx.GetRegister(_reg).AsLong;
			if (right == 0L) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong / right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ModLongInstruction : IInstruction
	{
		private readonly int _reg; public ModLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			long right = ctx.GetRegister(_reg).AsLong;
			if (right == 0L) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong % right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Bitwise
	public sealed class AndLongInstruction : IInstruction
	{
		private readonly int _reg; public AndLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong & ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class OrLongInstruction : IInstruction
	{
		private readonly int _reg; public OrLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong | ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class XorLongInstruction : IInstruction
	{
		private readonly int _reg; public XorLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong ^ ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ShlLongInstruction : IInstruction
	{
		private readonly int _reg; public ShlLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			// Зсув вимагає int за правилами C#
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong << ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ShrLongInstruction : IInstruction
	{
		private readonly int _reg; public ShrLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(ctx.Accumulator.AsLong >> ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Comparison
	public sealed class EqLongInstruction : IInstruction
	{
		private readonly int _reg; public EqLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsLong == ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class NeqLongInstruction : IInstruction
	{
		private readonly int _reg; public NeqLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsLong != ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GtLongInstruction : IInstruction
	{
		private readonly int _reg; public GtLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsLong > ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LtLongInstruction : IInstruction
	{
		private readonly int _reg; public LtLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsLong < ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GteLongInstruction : IInstruction
	{
		private readonly int _reg; public GteLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsLong >= ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LteLongInstruction : IInstruction
	{
		private readonly int _reg; public LteLongInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsLong <= ctx.GetRegister(_reg).AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Unary
	public sealed class NegateLongInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(-ctx.Accumulator.AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class BitNotLongInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromLong(~ctx.Accumulator.AsLong);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}