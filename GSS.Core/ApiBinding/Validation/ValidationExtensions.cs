using GSS.Core.ApiBinding.Builders.Abstractions;

namespace GSS.Core.ApiBinding.Validation
{
	public static class ValidationExtensions
	{
		public static ValidationBuilder<T> MustBeInRange<T>(this ValidationBuilder<T> builder, T min, T max) where T : struct, IComparable<T>
		{
			builder.Validators.Add(new RangeValidator<T>(min, max));
			return builder;
		}

		public static ValidationBuilder<T> MustNotBeNull<T>(this ValidationBuilder<T> builder) where T : class
		{
			builder.Validators.Add(new NotNullValidator<T>());
			return builder;
		}

		public static ValidationBuilder<T> MustBeValidEnum<T>(this ValidationBuilder<T> builder) where T : struct, Enum
		{
			builder.Validators.Add(new EnumDefinedValidator<T>());
			return builder;
		}

		public static ValidationBuilder<string> MustNotBeEmpty(this ValidationBuilder<string> builder)
		{
			builder.Validators.Add(new StringNotEmptyValidator());
			return builder;
		}

		public static ValidationBuilder<string> MaxLength(this ValidationBuilder<string> builder, int maxLength)
		{
			builder.Validators.Add(new StringLengthValidator(maxLength));
			return builder;
		}

		public static ValidationBuilder<T> MustBePositive<T>(this ValidationBuilder<T> builder) where T : struct, IComparable<T>
		{
			builder.Validators.Add(new NumberPositiveValidator<T>());
			return builder;
		}

		public static ValidationBuilder<T> MustBeNonNegative<T>(this ValidationBuilder<T> builder) where T : struct, IComparable<T>
		{
			builder.Validators.Add(new NumberNonNegativeValidator<T>());
			return builder;
		}

		public static ValidationBuilder<float> MustBeValidNumber(this ValidationBuilder<float> builder)
		{
			builder.Validators.Add(new FloatValidValidator());
			return builder;
		}

		// Property Extensions
		public static IPropertyConfigurator<T, TVal> MustBeInRange<T, TVal>(this IPropertyConfigurator<T, TVal> config, TVal min, TVal max) where TVal : struct, IComparable<TVal>
		{
			config.Validation.MustBeInRange(min, max);
			return config;
		}

		public static IPropertyConfigurator<T, string> MustNotBeEmpty<T>(this IPropertyConfigurator<T, string> config)
		{
			config.Validation.MustNotBeEmpty();
			return config;
		}

		public static IPropertyConfigurator<T, TVal> MustBeValidEnum<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : struct, Enum
		{
			config.Validation.MustBeValidEnum();
			return config;
		}

		public static IPropertyConfigurator<T, TVal> MustNotBeNull<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : class
		{
			config.Validation.MustNotBeNull();
			return config;
		}

		public static IPropertyConfigurator<T, TVal> MustBePositive<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : struct, IComparable<TVal>
		{
			config.Validation.MustBePositive();
			return config;
		}

		public static IPropertyConfigurator<T, TVal> MustBeNonNegative<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : struct, IComparable<TVal>
		{
			config.Validation.MustBeNonNegative();
			return config;
		}

		public static IPropertyConfigurator<T, float> MustBeValidNumber<T>(this IPropertyConfigurator<T, float> config)
		{
			config.Validation.MustBeValidNumber();
			return config;
		}

		public static IPropertyConfigurator<T, string> MaxLength<T>(this IPropertyConfigurator<T, string> config, int maxLength)
		{
			config.Validation.MaxLength(maxLength);
			return config;
		}
	}
}
