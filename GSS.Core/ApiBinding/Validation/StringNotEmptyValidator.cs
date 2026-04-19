using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class StringNotEmptyValidator : IValidator<string>
	{
		public void Validate(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new GssValidationException("String cannot be null, empty, or whitespace.");
		}
	}
}