using GSS.Kernel.Api.Builders;

namespace GSS.Kernel.Api.Validation
{
    public static class PropertyValidationExtensions
    {
        public static IPropertyConfigurator<T, TVal> MustBeInRange<T, TVal>(this IPropertyConfigurator<T, TVal> config, TVal min, TVal max) where TVal : struct, IComparable<TVal>
        {
            config.Validation.InRange(min, max); return config;
        }

        public static IPropertyConfigurator<T, TVal> MustBePositive<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : struct, IComparable<TVal>
        {
            config.Validation.Positive(); return config;
        }

        public static IPropertyConfigurator<T, TVal> MustBeNonNegative<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : struct, IComparable<TVal>
        {
            config.Validation.NonNegative(); return config;
        }

        public static IPropertyConfigurator<T, TVal> MustNotBeNull<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : class
        {
            config.Validation.NotNull(); return config;
        }

        public static IPropertyConfigurator<T, string> MustNotBeEmpty<T>(this IPropertyConfigurator<T, string> config)
        {
            config.Validation.NotEmpty(); return config;
        }

        public static IPropertyConfigurator<T, TVal> MustBeValidEnum<T, TVal>(this IPropertyConfigurator<T, TVal> config) where TVal : struct, Enum
        {
            config.Validation.ValidEnum(); return config;
        }
    }
}