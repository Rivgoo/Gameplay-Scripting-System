using GSS.Kernel.Execution;
using GSS.Kernel.Primitives;
using GSS.Runtime.Engine;

namespace GSS.Runtime.Memory
{
    public sealed class RuntimeExecutionContext
    {
        public ExecutableGraph Graph { get; private set; } = null!;
        public object TargetInstance { get; private set; } = null!;
        public IGlobalResolver? GlobalResolver { get; private set; }

        public int InstructionPointer;
        public Variant Accumulator;

        private Variant[] _registers = Array.Empty<Variant>();
        private Variant[] _argBuffer = Array.Empty<Variant>();
        private CallFrame[] _callStack = Array.Empty<CallFrame>();

        public int BaseRegisterIndex;
        private int _callStackCount;

        public void Initialize(ExecutableGraph graph, object targetInstance, IGlobalResolver? resolver)
        {
            Graph = graph;
            TargetInstance = targetInstance;
            GlobalResolver = resolver;
            
            InstructionPointer = 0;
            BaseRegisterIndex = 0;
            _callStackCount = 0;
            Accumulator = Variant.Null;

            EnsureCapacity(ref _registers, graph.RequiredRegisters);
            EnsureCapacity(ref _argBuffer, 16);
            EnsureCallStackCapacity(16);
        }

        public void Reset()
        {
            Graph = null!;
            TargetInstance = null!;
            GlobalResolver = null;
            Array.Clear(_registers, 0, _registers.Length);
            Array.Clear(_argBuffer, 0, _argBuffer.Length);
        }

        public ref Variant GetRegister(int index)
        {
            return ref _registers[BaseRegisterIndex + index];
        }

        public void SetArg(int index, in Variant value)
        {
            if (index >= _argBuffer.Length) EnsureCapacity(ref _argBuffer, index + 4);
            _argBuffer[index] = value;
        }

        public ReadOnlySpan<Variant> GetArgSpan(int count)
        {
            return new ReadOnlySpan<Variant>(_argBuffer, 0, count);
        }

        public void PushFrame(int returnIP, int requiredRegisters)
        {
            if (_callStackCount >= _callStack.Length) EnsureCallStackCapacity(_callStack.Length * 2);
            _callStack[_callStackCount++] = new CallFrame(returnIP, BaseRegisterIndex);
            
            BaseRegisterIndex += requiredRegisters;
            EnsureCapacity(ref _registers, BaseRegisterIndex + requiredRegisters);
        }

        public int PopFrame()
        {
            var frame = _callStack[--_callStackCount];
            BaseRegisterIndex = frame.BaseRegister;
            return frame.ReturnIP;
        }

        private static void EnsureCapacity(ref Variant[] array, int requiredSize)
        {
            if (array.Length < requiredSize) Array.Resize(ref array, Math.Max(array.Length * 2, requiredSize));
        }

        private void EnsureCallStackCapacity(int size)
        {
            if (_callStack.Length < size) Array.Resize(ref _callStack, size);
        }
    }
}