namespace GSS.Kernel.Api.Validation
{
    public sealed class ValidationBuilder<T>
    {
        public List<IValidator<T>> Validators { get; } = new();
    }
}