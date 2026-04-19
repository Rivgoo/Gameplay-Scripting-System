using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class NotNullValidator<T> : IValidator<T> where T : class
	{
		public void Validate(T value)
		{
			if (value == null)
				throw new GssValidationException("Value cannot be null.");
		}
	}
}