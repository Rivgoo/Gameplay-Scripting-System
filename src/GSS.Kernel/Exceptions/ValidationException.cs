namespace GSS.Kernel.Exceptions
{
    public sealed class ValidationException : KernelException
    {
        public ValidationException(string message) : base($"Validation Error: {message}") { }
    }
}