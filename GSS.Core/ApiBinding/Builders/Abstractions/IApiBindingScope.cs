using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders.Abstractions;

namespace GSS.Core.ApiBinding.Builders
{
	public interface IApiBindingScope<T>
	{
		IMethodConfigurator<T, VoidResult> Method(string name);
		IMethodConfigurator<T, TResult> Method<TResult>(string name);
		IPropertyConfigurator<T, TValue> Property<TValue>(string name);
	}
}