using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Execution;

namespace GSS.Core.Runtime.Memory
{
	public sealed class RuntimeExecutionContext
	{
		public object TargetInstance { get; private set; } = null!;
		public ExecutableGraph Graph { get; private set; } = null!;
		public IGlobalDependencyResolver? GlobalResolver { get; private set; }

		public int InstructionPointer;
		public GssValue Accumulator;
		public float StateTimer;
		public string? ErrorMessage { get; private set; }

		private GssValue[] _registers = Array.Empty<GssValue>();
		private GssValue[] _argBuffer = Array.Empty<GssValue>();

		public void Initialize(object instance, ExecutableGraph graph, IGlobalDependencyResolver? resolver = null)
		{
			TargetInstance = instance;
			Graph = graph;
			GlobalResolver = resolver;
			InstructionPointer = 0;
			StateTimer = 0f;
			ErrorMessage = null;
			Accumulator = GssValue.Null;

			EnsureCapacity(ref _registers, graph.RequiredRegisters);
			EnsureCapacity(ref _argBuffer, 16);
		}

		public void SetRegister(int index, GssValue value)
		{
			_registers[index] = value;
		}

		public GssValue GetRegister(int index)
		{
			return _registers[index];
		}

		public void SetArg(int index, GssValue value)
		{
			if (index >= _argBuffer.Length)
			{
				EnsureCapacity(ref _argBuffer, index + 8);
			}
			_argBuffer[index] = value;
		}

		public ReadOnlySpan<GssValue> GetArgBufferSpan(int count)
		{
			return new ReadOnlySpan<GssValue>(_argBuffer, 0, count);
		}

		public void SetError(string message)
		{
			ErrorMessage = message;
		}

		public T Resolve<T>()
		{
			if (GlobalResolver == null)
				throw new InvalidOperationException($"GlobalResolver is not assigned. Cannot resolve {typeof(T).Name}.");

			return GlobalResolver.Resolve<T>();
		}

		public void Reset()
		{
			TargetInstance = null!;
			Graph = null!;
			GlobalResolver = null;
			ErrorMessage = null;
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