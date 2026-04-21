namespace GSS.Runtime.Exceptions
{
    public sealed class DivideByZeroException : RuntimeException
    {
        public DivideByZeroException() : base("Attempted to divide by zero.") { }
    }
}