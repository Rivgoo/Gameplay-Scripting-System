namespace GSS.Kernel.Exceptions
{
    public sealed class VariantCastException : KernelException
    {
        public VariantCastException(string expectedType, string actualType)
            : base($"Invalid variant cast. Expected '{expectedType}', but found '{actualType}'.") { }
    }
}