namespace GSS.Core.ApiBinding.Abstractions
{
	public interface IClassMetadata
	{
		string Name { get; }
		IMethodMetadata RetrieveMethod(string methodName);
		IPropertyMetadata RetrieveProperty(string propertyName);
	}
}