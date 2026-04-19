using GSS.Core.Exceptions;

namespace GSS.Core.ApiBinding.Exceptions
{
	public class GssValidationException : GssException
	{
		public GssValidationException(string message) : base($"GSS Validation Error: {message}") { }
	}
}