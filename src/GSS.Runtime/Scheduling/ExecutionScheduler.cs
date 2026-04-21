using GSS.Kernel.Execution;
using GSS.Runtime.Engine;
using GSS.Runtime.Events;
using GSS.Runtime.Memory;

namespace GSS.Runtime.Scheduling
{
    public sealed class ExecutionScheduler
    {
        private readonly ContextPool _pool;
        private readonly List<RuntimeExecutionContext> _activeContexts;
        
        public IGlobalResolver? GlobalResolver { get; set; }
        public Action<Exception, RuntimeExecutionContext>? OnFaulted { get; set; }

        public ExecutionScheduler(ContextPool pool, int initialCapacity = 256)
        {
            _pool = pool;
            _activeContexts = new List<RuntimeExecutionContext>(initialCapacity);
        }

        public void Enqueue(ExecutableGraph graph, object targetInstance, EventPayload? payload = null)
        {
            var context = _pool.Rent();
            context.Initialize(graph, targetInstance, GlobalResolver);
            payload?.Inject(context);
            _activeContexts.Add(context);
        }

        public void Tick()
        {
            for (int i = _activeContexts.Count - 1; i >= 0; i--)
            {
                var context = _activeContexts[i];

                try
                {
                    var state = VirtualMachine.Execute(context);

                    if (state != ExecutionState.Running)
                    {
                        RemoveContextAt(i);
                    }
                }
                catch (Exception ex)
                {
                    OnFaulted?.Invoke(ex, context);
                    RemoveContextAt(i);
                }
            }
        }

        private void RemoveContextAt(int index)
        {
            var context = _activeContexts[index];
            _pool.Return(context);

            int lastIndex = _activeContexts.Count - 1;
            if (index != lastIndex) _activeContexts[index] = _activeContexts[lastIndex];
            _activeContexts.RemoveAt(lastIndex);
        }
    }
}