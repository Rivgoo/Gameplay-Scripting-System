namespace GSS.Runtime.Exceptions
{
    public sealed class InvalidCollectionException : RuntimeException
    {
        public InvalidCollectionException(string type)
            : base($"Object of type '{type}' is not a valid collection. It does not implement ICollection or IList.") { }
    }
}