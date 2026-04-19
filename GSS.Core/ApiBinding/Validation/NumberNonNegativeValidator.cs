using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class NumberNonNegativeValidator<T> : IValidator<T> where T : struct, IComparable<T>
	{
		public void Validate(T value)
		{
			if (value.CompareTo(default) < 0)
				throw new GssValidationException($"Value {value} must be non-negative (zero or greater).");
		}
	}
}