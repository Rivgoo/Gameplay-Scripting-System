using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Api.Validation;
using GSS.Kernel.Exceptions;
using GSS.Kernel.Primitives;

namespace GSS.Kernel.Api.Builders
{
	internal sealed class MethodMetadata<TInstance, TResult> : IMethodConfigurator<TInstance, TResult>, IMethodMetadata
	{
		public string Name { get; }
		public int ArgumentCount { get; private set; }
		public VariantType ReturnType { get; }

		private MemberInvoker? _invoker;

		public MethodMetadata(string name)
		{
			Name = name;
			ArgumentCount = 0;
			ReturnType = typeof(TResult) == typeof(VoidResult) ? VariantType.Null : VariantExtensions.GetVariantType(typeof(TResult));
		}

		public void AddArgCount() => ArgumentCount++;

		public void SetInvoker(MemberInvoker invoker) => _invoker = invoker;

		public IParamBuilder1<TInstance, T1, TResult> Param<T1>(string name, Action<ValidationBuilder<T1>>? validate = null)
		{
			ArgumentCount++;
			return new ParamBuilder1<TInstance, T1, TResult>(this, CompileValidator(validate));
		}

		public void Handler(Action<TInstance> action)
		{
			_invoker = (inst, args) => { action((TInstance)inst); return Variant.Null; };
		}

		public void Handler(Func<TInstance, TResult> func)
		{
			_invoker = (inst, args) => VariantExtensions.Pack(func((TInstance)inst));
		}

		public Variant Invoke(object instance, ReadOnlySpan<Variant> args)
		{
			if (args.Length != ArgumentCount) throw new BindingException($"Method '{Name}' expects {ArgumentCount} arguments, got {args.Length}.");
			if (_invoker == null) throw new BindingException($"Method '{Name}' is missing Handler compilation.");
			return _invoker(instance, args);
		}

		internal static Action<T>? CompileValidator<T>(Action<ValidationBuilder<T>>? setup)
		{
			if (setup == null) return null;
			var builder = new ValidationBuilder<T>();
			setup(builder);
			var validators = builder.Validators.ToArray();
			if (validators.Length == 0) return null;
			return val => { for (int i = 0; i < validators.Length; i++) validators[i].Validate(val); };
		}
	}
}