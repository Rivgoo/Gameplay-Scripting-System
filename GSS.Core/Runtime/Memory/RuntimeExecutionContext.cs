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
		private CallFrame[] _callStack = Array.Empty<CallFrame>();
		private int _callStackCount;
		private int _currentBaseRegisterIndex;

		public void Initialize(object instance, ExecutableGraph graph, IGlobalDependencyResolver? resolver = null)
		{
			TargetInstance = instance;
			Graph = graph;
			GlobalResolver = resolver;
			InstructionPointer = 0;
			StateTimer = 0f;
			ErrorMessage = null;
			Accumulator = GssValue.Null;
			_callStackCount = 0;
			_currentBaseRegisterIndex = 0;

			EnsureCapacity(ref _registers, graph.RequiredRegisters);
			EnsureCapacity(ref _argBuffer, 16);
			EnsureCallStackCapacity(16);
		}

		public ref GssValue GetRegisterRef(int index)
		{
			return ref _registers[_currentBaseRegisterIndex + index];
		}

		public void SetArg(int index, in GssValue value)
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

		public void PushFrame(int returnIp, int requestedRegisters)
		{
			if (_callStackCount >= _callStack.Length)
			{
				EnsureCallStackCapacity(_callStack.Length * 2);
			}

			_callStack[_callStackCount++] = new CallFrame(returnIp, _currentBaseRegisterIndex);
			_currentBaseRegisterIndex += requestedRegisters;

			EnsureCapacity(ref _registers, _currentBaseRegisterIndex + requestedRegisters);
		}

		public bool TryPopFrame(out int returnIp)
		{
			if (_callStackCount == 0)
			{
				returnIp = -1;
				return false;
			}

			var frame = _callStack[--_callStackCount];
			returnIp = frame.ReturnInstructionPointer;
			_currentBaseRegisterIndex = frame.BaseRegisterIndex;
			return true;
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
			_callStackCount = 0;
			_currentBaseRegisterIndex = 0;

			Array.Clear(_registers, 0, _registers.Length);
			Array.Clear(_argBuffer, 0, _argBuffer.Length);
		}

		private static void EnsureCapacity(ref GssValue[] array, int requiredSize)
		{
			if (array.Length < requiredSize)
			{
				Array.Resize(ref array, requiredSize * 2);
			}
		}

		private void EnsureCallStackCapacity(int requiredSize)
		{
			if (_callStack.Length < requiredSize)
			{
				Array.Resize(ref _callStack, requiredSize);
			}
		}
	}
}