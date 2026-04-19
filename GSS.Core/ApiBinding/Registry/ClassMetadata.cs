using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Exceptions;

namespace GSS.Core.ApiBinding.Registry
{
	internal class ClassMetadata : IClassMetadata
	{
		public string Name { get; }
		private readonly Dictionary<string, IMethodMetadata> _methods;
		private readonly Dictionary<string, IPropertyMetadata> _properties;

		public ClassMetadata(string name, Dictionary<string, IMethodMetadata> methods, Dictionary<string, IPropertyMetadata> props)
		{
			Name = name;
			_methods = methods;
			_properties = props;
		}

		public IMethodMetadata RetrieveMethod(string name)
		{
			if (!_methods.TryGetValue(name, out var meta))
				throw new GssMemberNotFoundException(name, $"Method not found in class '{Name}'.");

			return meta;
		}

		public IPropertyMetadata RetrieveProperty(string name)
		{
			if (!_properties.TryGetValue(name, out var meta))
				throw new GssMemberNotFoundException(name, $"Property not found in class '{Name}'.");

			return meta;
		}
	}
}