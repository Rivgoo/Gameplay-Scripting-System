using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Math
{
	public sealed class Vec2MagnitudeInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float x = ctx.Accumulator.X; float y = ctx.Accumulator.Y;
			ctx.Accumulator = GssValue.FromFloat(MathF.Sqrt(x * x + y * y));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec2SqrMagnitudeInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float x = ctx.Accumulator.X; float y = ctx.Accumulator.Y;
			ctx.Accumulator = GssValue.FromFloat(x * x + y * y);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec2NormalizeInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float x = ctx.Accumulator.X; float y = ctx.Accumulator.Y;
			float mag = MathF.Sqrt(x * x + y * y);
			if (mag > 1E-05f)
			{
				ctx.Accumulator.X = x / mag;
				ctx.Accumulator.Y = y / mag;
			}
			else
			{
				ctx.Accumulator.X = 0; ctx.Accumulator.Y = 0;
			}
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec2DotInstruction : IInstruction
	{
		private readonly int _reg; public Vec2DotInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var b = ctx.GetRegister(_reg);
			float dot = ctx.Accumulator.X * b.X + ctx.Accumulator.Y * b.Y;
			ctx.Accumulator = GssValue.FromFloat(dot);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec2DistanceInstruction : IInstruction
	{
		private readonly int _reg; public Vec2DistanceInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var b = ctx.GetRegister(_reg);
			float dx = ctx.Accumulator.X - b.X;
			float dy = ctx.Accumulator.Y - b.Y;
			ctx.Accumulator = GssValue.FromFloat(MathF.Sqrt(dx * dx + dy * dy));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec2ScaleInstruction : IInstruction
	{
		private readonly int _reg; public Vec2ScaleInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var b = ctx.GetRegister(_reg);
			ctx.Accumulator.X *= b.X;
			ctx.Accumulator.Y *= b.Y;
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec2LerpInstruction : IInstruction
	{
		private readonly int _targetReg, _tReg;
		public Vec2LerpInstruction(int targetReg, int tReg) { _targetReg = targetReg; _tReg = tReg; }
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float t = MathF.Max(0, MathF.Min(1, ctx.GetRegister(_tReg).AsFloat));
			var target = ctx.GetRegister(_targetReg);
			ctx.Accumulator.X += (target.X - ctx.Accumulator.X) * t;
			ctx.Accumulator.Y += (target.Y - ctx.Accumulator.Y) * t;
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
}