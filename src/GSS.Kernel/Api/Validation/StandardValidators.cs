using GSS.Kernel.Exceptions;

namespace GSS.Kernel.Api.Validation
{
    public sealed class RangeValidator<T> : IValidator<T> where T : struct, IComparable<T>
    {
        private readonly T _min, _max;
        public RangeValidator(T min, T max) { _min = min; _max = max; }
        public void Validate(T value)
        {
            if (value.CompareTo(_min) < 0 || value.CompareTo(_max) > 0)
                throw new ValidationException($"Value {value} is out of range [{_min}, {_max}].");
        }
    }

    public sealed class PositiveValidator<T> : IValidator<T> where T : struct, IComparable<T>
    {
        public void Validate(T value)
        {
            if (value.CompareTo(default) <= 0)
                throw new ValidationException($"Value {value} must be strictly positive.");
        }
    }

    public sealed class NonNegativeValidator<T> : IValidator<T> where T : struct, IComparable<T>
    {
        public void Validate(T value)
        {
            if (value.CompareTo(default) < 0)
                throw new ValidationException($"Value {value} must be non-negative.");
        }
    }

    public sealed class NotNullValidator<T> : IValidator<T> where T : class
    {
        public void Validate(T value)
        {
            if (value == null)
                throw new ValidationException("Value cannot be null.");
        }
    }

    public sealed class NotEmptyValidator : IValidator<string>
    {
        public void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException("String cannot be empty or whitespace.");
        }
    }

    public sealed class EnumValidator<T> : IValidator<T> where T : struct, Enum
    {
        public void Validate(T value)
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ValidationException($"Value '{value}' is not defined in enum '{typeof(T).Name}'.");
        }
    }

    public static class ValidationExtensions
    {
        public static ValidationBuilder<T> InRange<T>(this ValidationBuilder<T> b, T min, T max) where T : struct, IComparable<T> { b.Validators.Add(new RangeValidator<T>(min, max)); return b; }
        public static ValidationBuilder<T> Positive<T>(this ValidationBuilder<T> b) where T : struct, IComparable<T> { b.Validators.Add(new PositiveValidator<T>()); return b; }
        public static ValidationBuilder<T> NonNegative<T>(this ValidationBuilder<T> b) where T : struct, IComparable<T> { b.Validators.Add(new NonNegativeValidator<T>()); return b; }
        public static ValidationBuilder<T> NotNull<T>(this ValidationBuilder<T> b) where T : class { b.Validators.Add(new NotNullValidator<T>()); return b; }
        public static ValidationBuilder<string> NotEmpty(this ValidationBuilder<string> b) { b.Validators.Add(new NotEmptyValidator()); return b; }
        public static ValidationBuilder<T> ValidEnum<T>(this ValidationBuilder<T> b) where T : struct, Enum { b.Validators.Add(new EnumValidator<T>()); return b; }
    }
}