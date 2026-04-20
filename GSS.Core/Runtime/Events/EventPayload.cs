using GSS.Core.ApiBinding.Models;
using GSS.Core.Runtime.Events.Abstractions;
using GSS.Core.Runtime.Memory;

namespace GSS.Core.Runtime.Events
{
	public sealed class EventPayload : IEventPayload
	{
		private GssValue[] _args = Array.Empty<GssValue>();
		private int _maxIndex = -1;

		public EventPayload SetArgument(int registerIndex, in GssValue value)
		{
			if (registerIndex >= _args.Length)
			{
				Array.Resize(ref _args, registerIndex + 4);
			}

			_args[registerIndex] = value;
			if (registerIndex > _maxIndex)
			{
				_maxIndex = registerIndex;
			}

			return this;
		}

		public void WriteToContext(RuntimeExecutionContext context)
		{
			for (int i = 0; i <= _maxIndex; i++)
			{
				context.GetRegisterRef(i) = _args[i];
			}
		}
	}
}