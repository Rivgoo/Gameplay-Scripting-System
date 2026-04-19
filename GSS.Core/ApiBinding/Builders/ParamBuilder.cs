using GSS.Core.ApiBinding.Builders.Abstractions;
using GSS.Core.ApiBinding.Validation;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class ParamBuilder1<TInstance, T1, TResult> : IParamBuilder1<TInstance, T1, TResult>
	{
		private readonly MethodCore<TInstance, TResult> _core;
		public ParamBuilder1(MethodCore<TInstance, TResult> core) => _core = core;

		public IParamBuilder2<TInstance, T1, T2, TResult> Param<T2>(string name, Action<ValidationBuilder<T2>>? validate)
		{
			_core.AddParam(name, validate);
			return new ParamBuilder2<TInstance, T1, T2, TResult>(_core);
		}

		public void Handler(Action<TInstance, T1> action) => _core.Compile(action);
		public void Handler(Func<TInstance, T1, TResult> func) => _core.Compile(func);
	}

	internal sealed class ParamBuilder2<TInstance, T1, T2, TResult> : IParamBuilder2<TInstance, T1, T2, TResult>
	{
		private readonly MethodCore<TInstance, TResult> _core;
		public ParamBuilder2(MethodCore<TInstance, TResult> core) => _core = core;

		public IParamBuilder3<TInstance, T1, T2, T3, TResult> Param<T3>(string name, Action<ValidationBuilder<T3>>? validate)
		{
			_core.AddParam(name, validate);
			return new ParamBuilder3<TInstance, T1, T2, T3, TResult>(_core);
		}

		public void Handler(Action<TInstance, T1, T2> action) => _core.Compile(action);
		public void Handler(Func<TInstance, T1, T2, TResult> func) => _core.Compile(func);
	}

	internal sealed class ParamBuilder3<TInstance, T1, T2, T3, TResult> : IParamBuilder3<TInstance, T1, T2, T3, TResult>
	{
		private readonly MethodCore<TInstance, TResult> _core;
		public ParamBuilder3(MethodCore<TInstance, TResult> core) => _core = core;
		public IParamBuilder4<TInstance, T1, T2, T3, T4, TResult> Param<T4>(string name, Action<ValidationBuilder<T4>>? validate)
		{
			_core.AddParam(name, validate);
			return new ParamBuilder4<TInstance, T1, T2, T3, T4, TResult>(_core);
		}
		public void Handler(Action<TInstance, T1, T2, T3> action) => _core.Compile(action);
		public void Handler(Func<TInstance, T1, T2, T3, TResult> func) => _core.Compile(func);
	}

	internal sealed class ParamBuilder4<TInstance, T1, T2, T3, T4, TResult> : IParamBuilder4<TInstance, T1, T2, T3, T4, TResult>
	{
		private readonly MethodCore<TInstance, TResult> _core;
		public ParamBuilder4(MethodCore<TInstance, TResult> core) => _core = core;
		public IParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult> Param<T5>(string name, Action<ValidationBuilder<T5>>? validate)
		{
			_core.AddParam(name, validate);
			return new ParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult>(_core);
		}
		public void Handler(Action<TInstance, T1, T2, T3, T4> action) => _core.Compile(action);
		public void Handler(Func<TInstance, T1, T2, T3, T4, TResult> func) => _core.Compile(func);
	}

	internal sealed class ParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult> : IParamBuilder5<TInstance, T1, T2, T3, T4, T5, TResult>
	{
		private readonly MethodCore<TInstance, TResult> _core;
		public ParamBuilder5(MethodCore<TInstance, TResult> core) => _core = core;
		public void Handler(Action<TInstance, T1, T2, T3, T4, T5> action) => _core.Compile(action);
		public void Handler(Func<TInstance, T1, T2, T3, T4, T5, TResult> func) => _core.Compile(func);
	}
}