using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Maths
{
	#region Unary Math Functions
	public sealed class MathAbsInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Abs(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathAcosInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Acos(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathAsinInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Asin(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathAtanInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Atan(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathCeilInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Ceiling(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathCosInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Cos(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathExpInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Exp(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathFloorInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Floor(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathLog10Instruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Log10(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathRoundInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Round(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathSignInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Sign(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathSinInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Sin(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathSqrtInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Sqrt(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathTanInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Tan(ctx.Accumulator.AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Binary Math Functions
	public sealed class MathAtan2Instruction : IInstruction
	{
		private readonly int _xReg; public MathAtan2Instruction(int xReg) => _xReg = xReg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Atan2(ctx.Accumulator.AsFloat, ctx.GetRegister(_xReg).AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathMaxInstruction : IInstruction
	{
		private readonly int _reg; public MathMaxInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Max(ctx.Accumulator.AsFloat, ctx.GetRegister(_reg).AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathMinInstruction : IInstruction
	{
		private readonly int _reg; public MathMinInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Min(ctx.Accumulator.AsFloat, ctx.GetRegister(_reg).AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathPowInstruction : IInstruction
	{
		private readonly int _powReg; public MathPowInstruction(int powReg) => _powReg = powReg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			ctx.Accumulator = GssValue.FromFloat(MathF.Pow(ctx.Accumulator.AsFloat, ctx.GetRegister(_powReg).AsFloat));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathRepeatInstruction : IInstruction
	{
		private readonly int _lenReg; public MathRepeatInstruction(int lenReg) => _lenReg = lenReg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float t = ctx.Accumulator.AsFloat;
			float length = ctx.GetRegister(_lenReg).AsFloat;
			ctx.Accumulator = GssValue.FromFloat(MathF.Max(0, t - MathF.Floor(t / length) * length));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion

	#region Ternary Math Functions
	public sealed class MathClampInstruction : IInstruction
	{
		private readonly int _minReg, _maxReg;
		public MathClampInstruction(int minReg, int maxReg) { _minReg = minReg; _maxReg = maxReg; }
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float val = ctx.Accumulator.AsFloat;
			float min = ctx.GetRegister(_minReg).AsFloat;
			float max = ctx.GetRegister(_maxReg).AsFloat;
			ctx.Accumulator = GssValue.FromFloat(val < min ? min : (val > max ? max : val));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathLerpInstruction : IInstruction
	{
		private readonly int _aReg, _bReg;
		public MathLerpInstruction(int aReg, int bReg) { _aReg = aReg; _bReg = bReg; }
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float t = MathF.Max(0, MathF.Min(1, ctx.Accumulator.AsFloat)); // Clamp01
			float a = ctx.GetRegister(_aReg).AsFloat;
			float b = ctx.GetRegister(_bReg).AsFloat;
			ctx.Accumulator = GssValue.FromFloat(a + (b - a) * t);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class MathInverseLerpInstruction : IInstruction
	{
		private readonly int _aReg, _bReg;
		public MathInverseLerpInstruction(int aReg, int bReg) { _aReg = aReg; _bReg = bReg; }
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float val = ctx.Accumulator.AsFloat;
			float a = ctx.GetRegister(_aReg).AsFloat;
			float b = ctx.GetRegister(_bReg).AsFloat;
			if (a != b)
			{
				ctx.Accumulator = GssValue.FromFloat(MathF.Max(0, MathF.Min(1, (val - a) / (b - a))));
			}
			else
			{
				ctx.Accumulator = GssValue.FromFloat(0f);
			}
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	#endregion
}