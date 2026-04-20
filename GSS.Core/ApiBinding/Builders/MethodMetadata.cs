using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders.Abstractions;
using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Validation;
using GSS.Core.Runtime.Helpers;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class MethodMetadata<TInstance, TResult> : IMethodConfigurator<TInstance, TResult>, IMethodMetadata
	{
		private readonly MethodCore<TInstance, TResult> _core;

		public string Name => _core.Name;
		public int ArgumentCount => _core.Params.Count;
		public GssType ReturnType { get; }

		public MethodMetadata(string name)
		{
			_core = new MethodCore<TInstance, TResult>(name);
			ReturnType = GssValuePacker.GetGssType(typeof(TResult));
		}

		public IMethodConfigurator<TInstance, TResult> WithAlias(string alias)
		{
			_core.Name = alias;
			return this;
		}

		public IParamBuilder1<TInstance, T1, TResult> Param<T1>(string name, Action<ValidationBuilder<T1>>? validate = null)
		{
			_core.AddParam(name, validate);
			return new ParamBuilder1<TInstance, T1, TResult>(_core);
		}

		public void Handler(Action<TInstance> action)
		{
			_core.Compile(action);
		}

		public void Handler(Func<TInstance, TResult> func)
		{
			_core.Compile(func);
		}

		public GssValue Invoke(object instance, ReadOnlySpan<GssValue> args)
		{
			ValidateArgumentCount(args.Length);

			if (_core.Invoker == null)
			{
				throw new InvalidOperationException($"Method '{Name}' was not compiled. Call Handler() during registration.");
			}

			return _core.Invoker(instance, args);
		}

		private void ValidateArgumentCount(int actualCount)
		{
			if (actualCount != ArgumentCount)
			{
				throw new GssArgumentCountMismatchException(Name, ArgumentCount, actualCount);
			}
		}
	}
}