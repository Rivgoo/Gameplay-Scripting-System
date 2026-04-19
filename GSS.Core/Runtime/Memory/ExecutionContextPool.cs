using System.Collections.Concurrent;

namespace GSS.Core.Runtime.Memory
{
	public sealed class ExecutionContextPool
	{
		private readonly ConcurrentBag<RuntimeExecutionContext> _pool = new();

		public RuntimeExecutionContext Rent()
		{
			if (_pool.TryTake(out var context))
				return context;

			return new RuntimeExecutionContext();
		}

		public void Return(RuntimeExecutionContext context)
		{
			context.Reset();
			_pool.Add(context);
		}
	}
}