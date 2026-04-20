namespace GSS.Core.Runtime.Exceptions
{
	public sealed class GssIndexOutOfRangeException : GssRuntimeException
	{
		public GssIndexOutOfRangeException(int index, int length)
			: base($"Index was out of bounds. Index: {index}, Collection length: {length}.") { }
	}
}