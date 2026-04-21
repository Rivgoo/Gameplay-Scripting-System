using GSS.Kernel.Exceptions;

namespace GSS.Runtime.Exceptions
{
    public class RuntimeException : KernelException
    {
        public RuntimeException(string message) : base($"GSS Runtime Panic: {message}") { }
    }
}