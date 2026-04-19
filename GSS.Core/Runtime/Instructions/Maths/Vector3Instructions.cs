using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Enums;
using GSS.Core.Runtime.Instructions.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Instructions.Math
{
	public sealed class Vec3MagnitudeInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float x = ctx.Accumulator.X; float y = ctx.Accumulator.Y; float z = ctx.Accumulator.Z;
			ctx.Accumulator = GssValue.FromFloat(MathF.Sqrt(x * x + y * y + z * z));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec3SqrMagnitudeInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float x = ctx.Accumulator.X; float y = ctx.Accumulator.Y; float z = ctx.Accumulator.Z;
			ctx.Accumulator = GssValue.FromFloat(x * x + y * y + z * z);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec3NormalizeInstruction : IInstruction
	{
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float x = ctx.Accumulator.X; float y = ctx.Accumulator.Y; float z = ctx.Accumulator.Z;
			float mag = MathF.Sqrt(x * x + y * y + z * z);
			if (mag > 1E-05f)
			{
				ctx.Accumulator.X = x / mag;
				ctx.Accumulator.Y = y / mag;
				ctx.Accumulator.Z = z / mag;
			}
			else
			{
				ctx.Accumulator.X = 0; ctx.Accumulator.Y = 0; ctx.Accumulator.Z = 0;
			}
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec3DotInstruction : IInstruction
	{
		private readonly int _reg; public Vec3DotInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var b = ctx.GetRegister(_reg);
			float dot = ctx.Accumulator.X * b.X + ctx.Accumulator.Y * b.Y + ctx.Accumulator.Z * b.Z;
			ctx.Accumulator = GssValue.FromFloat(dot);
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec3CrossInstruction : IInstruction
	{
		private readonly int _reg; public Vec3CrossInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var a = ctx.Accumulator;
			var b = ctx.GetRegister(_reg);
			ctx.Accumulator.X = a.Y * b.Z - a.Z * b.Y;
			ctx.Accumulator.Y = a.Z * b.X - a.X * b.Z;
			ctx.Accumulator.Z = a.X * b.Y - a.Y * b.X;
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec3DistanceInstruction : IInstruction
	{
		private readonly int _reg; public Vec3DistanceInstruction(int reg) => _reg = reg;
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			var b = ctx.GetRegister(_reg);
			float dx = ctx.Accumulator.X - b.X;
			float dy = ctx.Accumulator.Y - b.Y;
			float dz = ctx.Accumulator.Z - b.Z;
			ctx.Accumulator = GssValue.FromFloat(MathF.Sqrt(dx * dx + dy * dy + dz * dz));
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
	public sealed class Vec3LerpInstruction : IInstruction
	{
		private readonly int _targetReg, _tReg;
		public Vec3LerpInstruction(int targetReg, int tReg) { _targetReg = targetReg; _tReg = tReg; }
		public ExecutionState Execute(RuntimeExecutionContext ctx)
		{
			float t = MathF.Max(0, MathF.Min(1, ctx.GetRegister(_tReg).AsFloat));
			var target = ctx.GetRegister(_targetReg);
			ctx.Accumulator.X += (target.X - ctx.Accumulator.X) * t;
			ctx.Accumulator.Y += (target.Y - ctx.Accumulator.Y) * t;
			ctx.Accumulator.Z += (target.Z - ctx.Accumulator.Z) * t;
			ctx.InstructionPointer++; return ExecutionState.Success;
		}
	}
}