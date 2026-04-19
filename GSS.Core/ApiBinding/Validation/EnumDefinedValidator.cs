using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public sealed class EnumDefinedValidator<T> : IValidator<T> where T : struct, Enum
	{
		public void Validate(T value)
		{
			if (!Enum.IsDefined(typeof(T), value))
				throw new GssValidationException($"Value '{value}' is not defined in enum '{typeof(T).Name}'.");
		}
	}
}