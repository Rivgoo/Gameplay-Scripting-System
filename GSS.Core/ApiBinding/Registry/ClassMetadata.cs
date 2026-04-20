using GSS.Core.ApiBinding.Abstractions;

namespace GSS.Core.ApiBinding.Registry
{
	public sealed class ClassMetadata : IClassMetadata
	{
		public string Name { get; }
		private readonly Dictionary<string, IMethodMetadata> _methods;
		private readonly Dictionary<string, IPropertyMetadata> _properties;

		public ClassMetadata(string name, Dictionary<string, IMethodMetadata> methods, Dictionary<string, IPropertyMetadata> properties)
		{
			Name = name;
			_methods = methods;
			_properties = properties;
		}

		public bool TryRetrieveMethod(string methodName, int argumentCount, out IMethodMetadata method)
		{
			if (_methods.TryGetValue(methodName, out var m) && m.ArgumentCount == argumentCount)
			{
				method = m;
				return true;
			}

			method = null!;
			return false;
		}

		public bool TryRetrieveProperty(string propertyName, out IPropertyMetadata property)
		{
			return _properties.TryGetValue(propertyName, out property!);
		}
	}
}