using GSS.Kernel.Primitives;

namespace GSS.Kernel.Api.Abstractions
{
    public interface IApiRegistry
    {
        IClassMetadata GetClass(string className);
    }

    public interface IClassMetadata
    {
        string Name { get; }
        bool TryGetMethod(string methodName, int argCount, out IMethodMetadata method);
        bool TryGetProperty(string propertyName, out IPropertyMetadata property);
    }

    public interface IMethodMetadata
    {
        string Name { get; }
        int ArgumentCount { get; }
        VariantType ReturnType { get; }
        Variant Invoke(object instance, ReadOnlySpan<Variant> args);
    }

    public interface IPropertyMetadata
    {
        string Name { get; }
        bool CanRead { get; }
        bool CanWrite { get; }
        VariantType Type { get; }
        Variant Read(object instance);
        void Write(object instance, Variant value);
    }
}