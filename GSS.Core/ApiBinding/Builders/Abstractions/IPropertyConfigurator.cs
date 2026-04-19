using GSS.Core.ApiBinding.Validation;

namespace GSS.Core.ApiBinding.Builders.Abstractions
{
	public interface IPropertyConfigurator<TInstance, TValue>
	{
		IPropertyConfigurator<TInstance, TValue> Get(Func<TInstance, TValue> getter);
		IPropertyConfigurator<TInstance, TValue> Set(Action<TInstance, TValue> setter);

		ValidationBuilder<TValue> Validation { get; }
	}
}