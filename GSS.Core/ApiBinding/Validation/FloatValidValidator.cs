using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class FloatValidValidator : IValidator<float>
	{
		public void Validate(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
				throw new GssValidationException($"Float value is mathematically invalid (NaN or Infinity).");
		}
	}
}