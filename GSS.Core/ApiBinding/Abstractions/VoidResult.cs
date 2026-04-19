namespace GSS.Core.ApiBinding.Abstractions
{
	public sealed class VoidResult
	{
		public static readonly VoidResult Instance = new();
		private VoidResult() { }
	}
}