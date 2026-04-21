using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Api.Builders;

namespace GSS.Kernel.Api.Registry
{
    public sealed class ApiRegistryBuilder
    {
        private readonly Dictionary<string, IClassMetadata> _classes = new();

        public ApiRegistryBuilder Register<T>(ApiProfile<T> profile)
        {
            var className = typeof(T).Name;
            var scope = new ApiBindingScope<T>();
            
            profile.Configure(scope);

            var metadata = new ClassMetadata(className, scope.Methods, scope.Properties);
            _classes[className] = metadata;
            
            return this;
        }

        public IApiRegistry Build() => new ApiRegistry(_classes);
    }
}