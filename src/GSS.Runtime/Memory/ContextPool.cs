namespace GSS.Runtime.Memory
{
    public sealed class ContextPool
    {
        private readonly Stack<RuntimeExecutionContext> _pool;

        public ContextPool(int initialCapacity = 64)
        {
            _pool = new Stack<RuntimeExecutionContext>(initialCapacity);
            for (int i = 0; i < initialCapacity; i++) _pool.Push(new RuntimeExecutionContext());
        }

        public RuntimeExecutionContext Rent()
        {
            return _pool.Count > 0 ? _pool.Pop() : new RuntimeExecutionContext();
        }

        public void Return(RuntimeExecutionContext context)
        {
            context.Reset();
            _pool.Push(context);
        }
    }
}