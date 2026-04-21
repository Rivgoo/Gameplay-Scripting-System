namespace GSS.Kernel.Exceptions
{
    public sealed class ArgumentCountMismatchException : KernelException
    {
        public ArgumentCountMismatchException(string methodName, int expected, int actual)
            : base($"Execution Error: Method '{methodName}' expects {expected} arguments, but received {actual}.") { }
    }
}