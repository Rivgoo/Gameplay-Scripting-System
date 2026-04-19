using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Builders.Abstractions;
using GSS.Core.ApiBinding.Exceptions;
using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Validation;
using GSS.Core.ApiBinding.Validation.Abstractions;

namespace GSS.Core.ApiBinding.Builders
{
	internal sealed class PropertyMetadata<TInstance, TValue> : IPropertyMetadata, IPropertyConfigurator<TInstance, TValue>
	{
		public string Name { get; }
		public bool CanRead => _getter != null;
		public bool CanWrite => _setter != null;

		private Func<TInstance, TValue>? _getter;
		private Action<TInstance, TValue>? _setter;
		private IValidator<TValue>[]? _validatorsCache;

		public ValidationBuilder<TValue> Validation { get; } = new();

		public PropertyMetadata(string name) => Name = name;

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
			if (_getter == null)
				throw new GssMemberNotFoundException(Name, "Property is write-only.");

			TValue val = _getter((TInstance)instance);

			if (val is float f) return GssValue.FromFloat(f);
			if (val is int i) return GssValue.FromInt(i);
			if (val is bool b) return GssValue.FromBool(b);
			if (val is double d) return GssValue.FromDouble(d);
			if (val is long l) return GssValue.FromLong(l);

			return GssValue.FromObject(val);
		}

		public void SetValue(object instance, GssValue value)
		{
			if (_setter == null)
				throw new GssMemberNotFoundException(Name, "Property is read-only.");

			TValue unboxed = value.Unbox<TValue>();

			if (_validatorsCache == null)
				_validatorsCache = Validation.Validators.ToArray();

			for (int i = 0; i < _validatorsCache.Length; i++)
			{
				_validatorsCache[i].Validate(unboxed);
			}

			_setter((TInstance)instance, unboxed);
		}
	}
}