using GSS.Kernel.Api.Validation;

namespace GSS.Kernel.Api.Builders
{
	public interface IPropertyConfigurator<TInstance, TValue>
	{
		IPropertyConfigurator<TInstance, TValue> Get(Func<TInstance, TValue> getter);
		IPropertyConfigurator<TInstance, TValue> Set(Action<TInstance, TValue> setter);
		ValidationBuilder<TValue> Validation { get; }
	}
}