namespace GSS.Kernel.Api.Builders
{
	public interface IApiBindingScope<T>
    {
        IMethodConfigurator<T, object> Method(string name);
        IMethodConfigurator<T, TResult> Method<TResult>(string name);
        IPropertyConfigurator<T, TValue> Property<TValue>(string name);
    }
}