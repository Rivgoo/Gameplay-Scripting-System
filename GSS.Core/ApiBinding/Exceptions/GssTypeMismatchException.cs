using GSS.Core.Exceptions;

namespace GSS.Core.ApiBinding.Exceptions
{
	public class GssTypeMismatchException : GssException
	{
		public GssTypeMismatchException(string expectedType, string actualType)
			: base($"GSS Type Error: Expected type '{expectedType}', but received '{actualType}'.") { }
	}
}