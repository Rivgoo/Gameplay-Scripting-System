using GSS.Core.Exceptions;

namespace GSS.Core.Runtime.Exceptions
{
	public class GssRuntimeException : GssException
	{
		public GssRuntimeException(string message) : base($"GSS Runtime Error: {message}") { }
	}
}