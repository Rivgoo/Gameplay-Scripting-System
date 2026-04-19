using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders.Abstractions;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class ApiBindingScope<T> : IApiBindingScope<T>
	{
		public readonly Dictionary<string, IMethodMetadata> Methods = new();
		public readonly Dictionary<string, IPropertyMetadata> Properties = new();

		public IMethodConfigurator<T, VoidResult> Method(string name)
		{
			var mb = new MethodMetadata<T, VoidResult>(name);
			Methods[name] = mb;
			return mb;
		}

		public IMethodConfigurator<T, TResult> Method<TResult>(string name)
		{
			var mb = new MethodMetadata<T, TResult>(name);
			Methods[name] = mb;
			return mb;
		}

		public IPropertyConfigurator<T, TValue> Property<TValue>(string name)
		{
			var pb = new PropertyMetadata<T, TValue>(name);
			Properties[name] = pb;
			return pb;
		}
	}
}