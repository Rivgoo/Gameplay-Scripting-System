namespace GSS.Core.Exceptions
{
	public abstract class GssException : Exception
	{
		protected GssException(string message) : base(message) { }
	}
}