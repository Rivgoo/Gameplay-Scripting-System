using GSS.Core.ApiBinding.Validation;

namespace GSS.Core.ApiBinding.Builders.Abstractions
{
	public interface IMethodConfigurator<TInstance, TResult>
	{
		IMethodConfigurator<TInstance, TResult> WithAlias(string alias);
		IParamBuilder1<TInstance, T1, TResult> Param<T1>(string name, Action<ValidationBuilder<T1>>? validate = null);
		void Handler(Action<TInstance> action);
		void Handler(Func<TInstance, TResult> func);
	}
}