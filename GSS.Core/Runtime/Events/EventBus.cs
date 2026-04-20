using GSS.Core.Runtime.Events.Abstractions;
using GSS.Core.Runtime.Execution;

namespace GSS.Core.Runtime.Events
{
	public sealed class EventBus : IEventBus
	{
		private readonly RuntimeScheduler _scheduler;
		private readonly Dictionary<string, List<ExecutableGraph>> _subscribers = new();

		public EventBus(RuntimeScheduler scheduler)
		{
			_scheduler = scheduler;
		}

		public void Subscribe(string eventName, ExecutableGraph graph)
		{
			if (!_subscribers.TryGetValue(eventName, out var list))
			{
				list = new List<ExecutableGraph>();
				_subscribers[eventName] = list;
			}

			if (!list.Contains(graph))
			{
				list.Add(graph);
			}
		}

		public void Unsubscribe(string eventName, ExecutableGraph graph)
		{
			if (_subscribers.TryGetValue(eventName, out var list))
			{
				list.Remove(graph);
			}
		}

		public void Dispatch(string eventName, object targetInstance, IEventPayload? payload = null)
		{
			if (!_subscribers.TryGetValue(eventName, out var graphs))
				return;

			for (int i = 0; i < graphs.Count; i++)
			{
				_scheduler.Enqueue(targetInstance, graphs[i], payload);
			}
		}
	}
}