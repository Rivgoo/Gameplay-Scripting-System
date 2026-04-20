using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders;

namespace GSS.Core.ApiBinding.Registry
{
	public sealed class ApiRegistryBuilder
	{
		private readonly Dictionary<string, IClassMetadata> _classes = new();

		public void Register<T>(ApiProfile<T> profile)
		{
			var name = typeof(T).Name;
			var scope = new ApiBindingScope<T>();

			profile.Build(scope);

			_classes[name] = new ClassMetadata(name, scope.Methods, scope.Properties);
		}

		public IApiRegistry Build() => new ApiRegistry(_classes);
	}
}