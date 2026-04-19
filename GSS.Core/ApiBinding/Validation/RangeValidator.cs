using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class RangeValidator<T> : IValidator<T> where T : struct, IComparable<T>
	{
		private readonly T _min;
		private readonly T _max;

		public RangeValidator(T min, T max)
		{
			_min = min;
			_max = max;
		}

		public void Validate(T value)
		{
			if (value.CompareTo(_min) < 0 || value.CompareTo(_max) > 0)
				throw new GssValidationException($"Value {value} is out of range [{_min}, {_max}].");
		}
	}
}