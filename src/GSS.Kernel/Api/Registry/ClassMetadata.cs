using GSS.Kernel.Api.Abstractions;

namespace GSS.Kernel.Api.Registry
{
    internal sealed class ClassMetadata : IClassMetadata
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

        public bool TryGetMethod(string methodName, int argCount, out IMethodMetadata method)
        {
            if (_methods.TryGetValue(methodName, out var m) && m.ArgumentCount == argCount)
            {
                method = m;
                return true;
            }
            method = null!;
            return false;
        }

        public bool TryGetProperty(string propertyName, out IPropertyMetadata property)
        {
            return _properties.TryGetValue(propertyName, out property!);
        }
    }
}