using GSS.Kernel.Primitives;

namespace GSS.Kernel.Api.Abstractions
{
    public delegate Variant MemberInvoker(object instance, ReadOnlySpan<Variant> args);
}