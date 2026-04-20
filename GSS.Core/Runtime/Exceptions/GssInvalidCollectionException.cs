namespace GSS.Core.Runtime.Exceptions
{
	public sealed class GssInvalidCollectionException : GssRuntimeException
	{
		public GssInvalidCollectionException(string type)
			: base($"Object of type '{type}' is not a valid collection. It does not implement ICollection or IList.") { }
	}
}