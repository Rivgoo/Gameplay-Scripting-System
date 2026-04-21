using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Api.Validation;
using GSS.Kernel.Exceptions;
using GSS.Kernel.Primitives;

namespace GSS.Kernel.Api.Builders
{
	internal sealed class PropertyMetadata<TInstance, TValue> : IPropertyConfigurator<TInstance, TValue>, IPropertyMetadata
	{
		public string Name { get; }
		public bool CanRead => _getter != null;
		public bool CanWrite => _setter != null;
		public VariantType Type { get; }

		private Func<TInstance, TValue>? _getter;
		private Action<TInstance, TValue>? _setter;
		private IValidator<TValue>[]? _compiledValidators;

		public ValidationBuilder<TValue> Validation { get; } = new();

		public PropertyMetadata(string name)
		{
			Name = name;
			Type = VariantExtensions.GetVariantType(typeof(TValue));
		}

		public IPropertyConfigurator<TInstance, TValue> Get(Func<TInstance, TValue> getter) { _getter = getter; return this; }
		public IPropertyConfigurator<TInstance, TValue> Set(Action<TInstance, TValue> setter) { _setter = setter; return this; }

		public Variant Read(object instance)
		{
			if (_getter == null) throw new BindingException($"Property '{Name}' is write-only.");
			return VariantExtensions.Pack(_getter((TInstance)instance));
		}

		public void Write(object instance, Variant value)
		{
			if (_setter == null) throw new BindingException($"Property '{Name}' is read-only.");
			TValue unboxed = value.Unbox<TValue>();

			_compiledValidators ??= Validation.Validators.ToArray();
			for (int i = 0; i < _compiledValidators.Length; i++) _compiledValidators[i].Validate(unboxed);

			_setter((TInstance)instance, unboxed);
		}
	}
}