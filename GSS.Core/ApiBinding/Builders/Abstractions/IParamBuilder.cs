using GSS.Core.ApiBinding.Validation;

namespace GSS.Core.ApiBinding.Builders.Abstractions
{
	public interface IParamBuilder1<TInstance, T1, TResult>
	{
		IParamBuilder2<TInstance, T1, T2, TResult> Param<T2>(string name, Action<ValidationBuilder<T2>>? validate = null);
		void Handler(Action<TInstance, T1> action);
		void Handler(Func<TInstance, T1, TResult> func);
	}

	public interface IParamBuilder2<TInstance, T1, T2, TResult>
	{
		IParamBuilder3<TInstance, T1, T2, T3, TResult> Param<T3>(string name, Action<ValidationBuilder<T3>>? validate = null);
		void Handler(Action<TInstance, T1, T2> action);
		void Handler(Func<TInstance, T1, T2, TResult> func);
	}

	public interface IParamBuilder3<TInstance, T1, T2, T3, TResult>
	{
		IParamBuilder4<TInstance, T1, T2, T3, T4, TResult> Param<T4>(string name, Action<ValidationBuilder<T4>>? validate = null);
		void Handler(Action<TInstance, T1, T2, T3> action);
		void Handler(Func<TInstance, T1, T2, T3, TResult> func);
	}

	public interface IParamBuilder4<TInstance, T1, T2, T3, T4, TResult>
	{
		IParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult> Param<T5>(string name, Action<ValidationBuilder<T5>>? validate = null);
		void Handler(Action<TInstance, T1, T2, T3, T4> action);
		void Handler(Func<TInstance, T1, T2, T3, T4, TResult> func);
	}

	public interface IParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult>
	{
		void Handler(Action<TInstance, T1, T2, T3, T4, T5> action);
		void Handler(Func<TInstance, T1, T2, T3, T4, T5, TResult> func);
	}
}