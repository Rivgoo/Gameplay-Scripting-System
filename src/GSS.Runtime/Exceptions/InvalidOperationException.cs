namespace GSS.Runtime.Exceptions
{
    public sealed class InvalidOperationException : RuntimeException
    {
        public InvalidOperationException(string operation, string type)
            : base($"Operation '{operation}' is not valid for type '{type}'.") { }
    }
}