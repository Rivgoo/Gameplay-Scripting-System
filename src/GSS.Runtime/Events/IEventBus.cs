using GSS.Kernel.Execution;

namespace GSS.Runtime.Events
{
    public interface IEventBus
    {
        void Subscribe(string eventName, ExecutableGraph graph);
        void Unsubscribe(string eventName, ExecutableGraph graph);
        void Dispatch(string eventName, object targetInstance, EventPayload? payload = null);
    }
}