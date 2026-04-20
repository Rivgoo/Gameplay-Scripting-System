using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Events.Abstractions
{
	public interface IEventPayload
	{
		void WriteToContext(RuntimeExecutionContext context);
	}
}