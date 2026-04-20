namespace GSS.Core.ApiBinding.Abstractions
{
	public interface IClassMetadata
	{
		string Name { get; }
		bool TryRetrieveMethod(string methodName, int argumentCount, out IMethodMetadata method);
		bool TryRetrieveProperty(string propertyName, out IPropertyMetadata property);
	}
}