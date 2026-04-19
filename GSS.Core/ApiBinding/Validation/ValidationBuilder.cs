using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class ValidationBuilder<T>
	{
		public List<IValidator<T>> Validators { get; } = new();
	}
}