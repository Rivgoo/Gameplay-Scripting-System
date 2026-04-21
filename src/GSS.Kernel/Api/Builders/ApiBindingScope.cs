using GSS.Kernel.Api.Abstractions;

namespace GSS.Kernel.Api.Builders
{
    public sealed class ApiBindingScope<T> : IApiBindingScope<T>
    {
        public readonly Dictionary<string, IMethodMetadata> Methods = new();
        public readonly Dictionary<string, IPropertyMetadata> Properties = new();

        public IMethodConfigurator<T, object> Method(string name)
        {
            var builder = new MethodMetadata<T, object>(name);
            Methods[name] = builder;
            return builder;
        }

        public IMethodConfigurator<T, TResult> Method<TResult>(string name)
        {
            var builder = new MethodMetadata<T, TResult>(name);
            Methods[name] = builder;
            return builder;
        }

        public IPropertyConfigurator<T, TValue> Property<TValue>(string name)
        {
            var builder = new PropertyMetadata<T, TValue>(name);
            Properties[name] = builder;
            return builder;
        }
    }
}