namespace GSS.Core.ApiBinding.Abstractions
{
	public interface IApiRegistry
	{
		IClassMetadata RetrieveClass(string className);
	}
}