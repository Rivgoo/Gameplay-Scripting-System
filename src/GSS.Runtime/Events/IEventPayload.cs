using GSS.Runtime.Memory;

namespace GSS.Runtime.Events
{
    public interface IEventPayload
    {
        void Inject(RuntimeExecutionContext context);
    }
}