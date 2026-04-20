namespace GSS.Core.Runtime.Memory
{
	public sealed class ExecutionContextPool
	{
		private readonly Stack<RuntimeExecutionContext> _pool;

		public ExecutionContextPool(int initialCapacity = 64)
		{
			_pool = new Stack<RuntimeExecutionContext>(initialCapacity);
			for (int i = 0; i < initialCapacity; i++)
			{
				_pool.Push(new RuntimeExecutionContext());
			}
		}

		public RuntimeExecutionContext Rent()
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop();
			}

			return new RuntimeExecutionContext();
		}

		public void Return(RuntimeExecutionContext context)
		{
			context.Reset();
			_pool.Push(context);
		}
	}
}