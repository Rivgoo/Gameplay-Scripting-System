using GSS.Kernel.Api.Builders;

namespace GSS.Kernel.Api.Abstractions
{
    public abstract class ApiProfile<T>
    {
        public abstract void Configure(IApiBindingScope<T> scope);
    }
}