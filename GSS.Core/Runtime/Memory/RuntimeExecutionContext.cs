using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Execution;

namespace GSS.Core.Runtime.Memory
{
	public sealed class RuntimeExecutionContext
	{
		public object TargetInstance { get; private set; } = null!;
		public ExecutableGraph Graph { get; private set; } = null!;

		public int InstructionPointer;
		public GssValue Accumulator;
		public float StateTimer;

		private GssValue[] _registers = Array.Empty<GssValue>();
		private GssValue[] _argBuffer = Array.Empty<GssValue>();

		public void Initialize(object instance, ExecutableGraph graph)
		{
			TargetInstance = instance;
			Graph = graph;
			InstructionPointer = 0;
			StateTimer = 0f;
			Accumulator = GssValue.Null;

			EnsureCapacity(ref _registers, graph.VariableCount);
			EnsureCapacity(ref _argBuffer, graph.MaxArgumentCount);
		}

		public void SetRegister(int index, GssValue value)
		{
			_registers[index] = value;
		}

		public GssValue GetRegister(int index)
		{
			return _registers[index];
		}

		public Span<GssValue> GetArgBuffer(int count)
		{
			return _argBuffer.AsSpan(0, count);
		}

		public void Reset()
		{
			TargetInstance = null!;
			Graph = null!;
			InstructionPointer = 0;
			StateTimer = 0f;
			Accumulator = GssValue.Null;

			Array.Clear(_registers);
			Array.Clear(_argBuffer);
		}

		private static void EnsureCapacity(ref GssValue[] array, int requiredSize)
		{
			if (array.Length < requiredSize)
			{
				array = new GssValue[requiredSize];
			}
		}
	}
}