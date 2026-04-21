namespace GSS.Kernel.Api.Validation
{
    public interface IValidator<T>
    {
        void Validate(T value);
    }
}