namespace GSS.Kernel.Exceptions
{
    public sealed class BindingException : KernelException
    {
        public BindingException(string message) : base($"API Binding Error: {message}") { }
    }
}