using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Events.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Events
{
	public sealed class EventPayload : IEventPayload
	{
		private readonly Dictionary<int, GssValue> _args = new();

		public EventPayload SetArgument(int registerIndex, GssValue value)
		{
			_args[registerIndex] = value;
			return this;
		}

		public void WriteToContext(RuntimeExecutionContext context)
		{
			foreach (var arg in _args)
			{
				context.SetRegister(arg.Key, arg.Value);
			}
		}
	}
}