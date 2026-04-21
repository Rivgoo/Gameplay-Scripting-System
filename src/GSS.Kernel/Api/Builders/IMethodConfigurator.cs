using GSS.Kernel.Api.Validation;

namespace GSS.Kernel.Api.Builders
{
	public interface IMethodConfigurator<TInstance, TResult>
	{
		IParamBuilder1<TInstance, T1, TResult> Param<T1>(string name, Action<ValidationBuilder<T1>>? validate = null);
		void Handler(Action<TInstance> action);
		void Handler(Func<TInstance, TResult> func);
	}
}