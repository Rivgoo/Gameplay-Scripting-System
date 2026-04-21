namespace GSS.Runtime.Exceptions
{
	public sealed class OutOfBoundsException : RuntimeException
    {
        public OutOfBoundsException(int index, int length) 
            : base($"Index out of bounds. Tried to access {index}, but length is {length}.") { }
    }
}