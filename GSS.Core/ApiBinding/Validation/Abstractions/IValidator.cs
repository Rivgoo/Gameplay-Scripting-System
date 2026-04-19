namespace GSS.Core.ApiBinding.Validation.Abstractions
{
	public interface IValidator<T>
	{
		void Validate(T value);
	}
}