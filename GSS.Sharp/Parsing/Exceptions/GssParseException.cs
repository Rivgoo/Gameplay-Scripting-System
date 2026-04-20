using GSS.Core.Exceptions;
namespace GSS.Sharp.Parsing.Exceptions
{
	public sealed class GssParseException : GssException
	{
		public GssParseException(string message) : base($"Syntax Panic: {message}") { }
	}
}