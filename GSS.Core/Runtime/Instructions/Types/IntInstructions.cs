using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Exceptions;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Types
{
	#region Arithmetic
	public sealed class AddIntInstruction : IInstruction
	{
		private readonly int _reg; public AddIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt + ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class SubIntInstruction : IInstruction
	{
		private readonly int _reg; public SubIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt - ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MulIntInstruction : IInstruction
	{
		private readonly int _reg; public MulIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt * ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class DivIntInstruction : IInstruction
	{
		private readonly int _reg; public DivIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			int right = ctx.GetRegister(_reg).AsInt;
			if (right == 0) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt / right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ModIntInstruction : IInstruction
	{
		private readonly int _reg; public ModIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			int right = ctx.GetRegister(_reg).AsInt;
			if (right == 0) throw new GssDivideByZeroException();
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt % right);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Bitwise
	public sealed class AndIntInstruction : IInstruction
	{
		private readonly int _reg; public AndIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt & ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class OrIntInstruction : IInstruction
	{
		private readonly int _reg; public OrIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt | ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class XorIntInstruction : IInstruction
	{
		private readonly int _reg; public XorIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt ^ ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ShlIntInstruction : IInstruction
	{
		private readonly int _reg; public ShlIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt << ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class ShrIntInstruction : IInstruction
	{
		private readonly int _reg; public ShrIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(ctx.Accumulator.AsInt >> ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Comparison
	public sealed class EqIntInstruction : IInstruction
	{
		private readonly int _reg; public EqIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt == ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class NeqIntInstruction : IInstruction
	{
		private readonly int _reg; public NeqIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt != ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GtIntInstruction : IInstruction
	{
		private readonly int _reg; public GtIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt > ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LtIntInstruction : IInstruction
	{
		private readonly int _reg; public LtIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt < ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class GteIntInstruction : IInstruction
	{
		private readonly int _reg; public GteIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt >= ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class LteIntInstruction : IInstruction
	{
		private readonly int _reg; public LteIntInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromBool(ctx.Accumulator.AsInt <= ctx.GetRegister(_reg).AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Unary
	public sealed class NegateIntInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(-ctx.Accumulator.AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class BitNotIntInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromInt(~ctx.Accumulator.AsInt);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}