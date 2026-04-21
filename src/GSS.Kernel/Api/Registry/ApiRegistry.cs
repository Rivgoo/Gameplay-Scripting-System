using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Exceptions;

namespace GSS.Kernel.Api.Registry
{
    public sealed class ApiRegistry : IApiRegistry
    {
        private readonly Dictionary<string, IClassMetadata> _classes;

        public ApiRegistry(Dictionary<string, IClassMetadata> classes)
        {
            _classes = classes;
        }

        public IClassMetadata GetClass(string className)
        {
            if (!_classes.TryGetValue(className, out var meta))
                throw new BindingException($"Class '{className}' is not registered in API Registry.");
            return meta;
        }
    }
}