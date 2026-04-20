using GSS.Core.Runtime.Execution;

namespace GSS.Core.Runtime.Events.Abstractions
{
	public interface IEventBus
	{
		void Subscribe(string eventName, ExecutableGraph graph);
		void Unsubscribe(string eventName, ExecutableGraph graph);
		void Dispatch(string eventName, object targetInstance, IEventPayload? payload = null);
	}
}