using GSS.Kernel.Primitives;
using GSS.Runtime.Memory;

namespace GSS.Runtime.Events
{
    public sealed class EventPayload : IEventPayload
    {
        private Variant[] _args = Array.Empty<Variant>();
        private int _maxIndex = -1;

        public EventPayload Set(int registerIndex, in Variant value)
        {
            if (registerIndex >= _args.Length) Array.Resize(ref _args, registerIndex + 4);
            _args[registerIndex] = value;
            if (registerIndex > _maxIndex) _maxIndex = registerIndex;
            return this;
        }

        public void Inject(RuntimeExecutionContext context)
        {
            for (int i = 0; i <= _maxIndex; i++)
            {
                context.GetRegister(i) = _args[i];
            }
        }
    }
}