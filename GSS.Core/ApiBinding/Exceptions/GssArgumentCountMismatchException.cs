using GSS.Core.Exceptions;

namespace GSS.Core.ApiBinding.Exceptions
{
	public class GssArgumentCountMismatchException : GssException
	{
		public GssArgumentCountMismatchException(string methodName, int expected, int actual)
			: base($"GSS Execution Error: Method '{methodName}' expects {expected} arguments, but received {actual}.") { }
	}
}