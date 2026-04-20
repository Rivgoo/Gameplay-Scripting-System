using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders.Abstractions;
using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Validation;
using GSS.Core.ApiBinding.Validation.Abstractions;
using GSS.Core.Runtime.Helpers;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class PropertyMetadata<TInstance, TValue> : IPropertyMetadata, IPropertyConfigurator<TInstance, TValue>
	{
		public string Name { get; }
		public bool CanRead => _getter != null;
		public bool CanWrite => _setter != null;
		public GssType Type { get; }

		private Func<TInstance, TValue>? _getter;
		private Action<TInstance, TValue>? _setter;
		private IValidator<TValue>[]? _validatorsCache;

		public ValidationBuilder<TValue> Validation { get; } = new();

		public PropertyMetadata(string name)
		{
			Name = name;
			Type = GssValuePacker.GetGssType(typeof(TValue));
		}

		public IPropertyConfigurator<TInstance, TValue> Get(Func<TInstance, TValue> getter)
		{
			_getter = getter;
			return this;
		}

		public IPropertyConfigurator<TInstance, TValue> Set(Action<TInstance, TValue> setter)
		{
			_setter = setter;
			return this;
		}

		public GssValue GetValue(object instance)
		{
			if (_getter == null) throw new GssMemberNotFoundException(Name, "Property is write-only.");
			return GssValuePacker.Pack(_getter((TInstance)instance));
		}

		public void SetValue(object instance, GssValue value)
		{
			if (_setter == null) throw new GssMemberNotFoundException(Name, "Property is read-only.");
			TValue unboxed = value.Unbox<TValue>();

			_validatorsCache ??= Validation.Validators.ToArray();
			for (int i = 0; i < _validatorsCache.Length; i++) _validatorsCache[i].Validate(unboxed);

			_setter((TInstance)instance, unboxed);
		}
	}
}