using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class StringLengthValidator : IValidator<string>
	{
		private readonly int _maxLength;

		public StringLengthValidator(int maxLength)
		{
			_maxLength = maxLength;
		}

		public void Validate(string value)
		{
			if (value != null && value.Length > _maxLength)
				throw new GssValidationException($"String length ({value.Length}) exceeds the maximum allowed length of {_maxLength}.");
		}
	}
}