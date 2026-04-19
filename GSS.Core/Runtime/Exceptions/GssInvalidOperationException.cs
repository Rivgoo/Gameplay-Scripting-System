namespace GSS.Core.Runtime.Exceptions
{
	public sealed class GssInvalidOperationException : GssRuntimeException
	{
		public GssInvalidOperationException(string operation, string type)
			: base($"Operation '{operation}' is not valid for type '{type}'.") { }
	}
}