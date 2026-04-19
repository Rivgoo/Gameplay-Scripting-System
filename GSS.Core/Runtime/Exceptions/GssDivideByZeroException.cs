namespace GSS.Core.Runtime.Exceptions
{
	public sealed class GssDivideByZeroException : GssRuntimeException
	{
		public GssDivideByZeroException() : base("Attempted to divide by zero.") { }
	}
}