using GSS.Kernel.Exceptions;

namespace GSS.Sharp.Parsing.Exceptions
{
    public sealed class ParseException : KernelException
    {
        public ParseException(string message) : base($"Syntax Panic: {message}") { }
    }
}