using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders.Abstractions;
using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Validation;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class MethodMetadata<TInstance, TResult> : IMethodConfigurator<TInstance, TResult>, IMethodMetadata
	{
		private readonly MethodCore<TInstance, TResult> _core;
		public string Name => _core.Name;

		public MethodMetadata(string name) => _core = new MethodCore<TInstance, TResult>(name);

		public IMethodConfigurator<TInstance, TResult> WithAlias(string alias)
		{
			_core.Name = alias;
			return this;
		}

		public IParamBuilder1<TInstance, T1, TResult> Param<T1>(string name, Action<ValidationBuilder<T1>>? validate)
		{
			_core.AddParam(name, validate);
			return new ParamBuilder1<TInstance, T1, TResult>(_core);
		}

		public void Handler(Action<TInstance> action) => _core.Compile(action);
		public void Handler(Func<TInstance, TResult> func) => _core.Compile(func);

		public GssValue Invoke(object instance, ReadOnlySpan<GssValue> args)
		{
			if (args.Length != _core.Params.Count)
				throw new GssArgumentCountMismatchException(Name, _core.Params.Count, args.Length);

			return _core.Invoker!(instance, args);
		}
	}
}