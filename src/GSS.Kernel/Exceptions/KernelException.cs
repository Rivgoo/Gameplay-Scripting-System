namespace GSS.Kernel.Exceptions
{
    public abstract class KernelException : Exception
    {
        protected KernelException(string message) : base(message) { }
    }
}