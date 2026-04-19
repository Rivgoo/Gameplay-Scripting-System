using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class NumberPositiveValidator<T> : IValidator<T> where T : struct, IComparable<T>
	{
		public void Validate(T value)
		{
			if (value.CompareTo(default) <= 0)
				throw new GssValidationException($"Value {value} must be strictly positive (greater than zero).");
		}
	}
}