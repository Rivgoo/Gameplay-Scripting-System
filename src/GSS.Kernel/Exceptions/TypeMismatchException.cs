namespace GSS.Kernel.Exceptions
{
    public sealed class TypeMismatchException : KernelException
    {
        public TypeMismatchException(string expectedType, string actualType)
            : base($"Type Error: Expected '{expectedType}', but received '{actualType}'.") { }
    }
}