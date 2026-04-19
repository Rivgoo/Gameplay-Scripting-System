using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Exceptions;

namespace GSS.Core.ApiBinding.Registry
{
	internal class ApiRegistry : IApiRegistry
	{
		private readonly Dictionary<string, IClassMetadata> _classes;

		public ApiRegistry(Dictionary<string, IClassMetadata> classes) => _classes = classes;

		public IClassMetadata RetrieveClass(string name)
		{
			if (!_classes.TryGetValue(name, out var meta))
				throw new GssMemberNotFoundException(name, "Class not found in registry.");

			return meta;
		}
	}
}