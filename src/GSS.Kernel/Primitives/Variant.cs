using System.Runtime.InteropServices;
using GSS.Kernel.Exceptions;

namespace GSS.Kernel.Primitives
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Variant
    {
        [FieldOffset(0)] public VariantType Type;

        [FieldOffset(4)] public int AsInt;
        [FieldOffset(4)] public float AsFloat;
        [FieldOffset(4)] public bool AsBool;
        
        [FieldOffset(4)] public float X;
        [FieldOffset(8)] public float Y;
        [FieldOffset(12)] public float Z;
        [FieldOffset(16)] public float W;

        [FieldOffset(24)] public object? AsObject;

        public static Variant Null => new() { Type = VariantType.Null };
        public static Variant FromInt(int value) => new() { Type = VariantType.Int, AsInt = value };
        public static Variant FromFloat(float value) => new() { Type = VariantType.Float, AsFloat = value };
        public static Variant FromBool(bool value) => new() { Type = VariantType.Bool, AsBool = value };
        public static Variant FromObject(object? value) => new() { Type = VariantType.Object, AsObject = value };
        public static Variant FromVector(float x, float y, float z, float w = 0f) => new() { Type = VariantType.Vector, X = x, Y = y, Z = z, W = w };

        public readonly T Unbox<T>()
        {
            Type targetType = typeof(T);

            if (targetType == typeof(int)) { Ensure(VariantType.Int); return (T)(object)AsInt; }
            if (targetType == typeof(float)) { Ensure(VariantType.Float); return (T)(object)AsFloat; }
            if (targetType == typeof(bool)) { Ensure(VariantType.Bool); return (T)(object)AsBool; }
            
            Ensure(VariantType.Object);

            if (AsObject == null)
            {
                if (default(T) == null) return default!;
                throw new VariantCastException(targetType.Name, "Null");
            }

            if (AsObject is T typedObject) return typedObject;

            throw new VariantCastException(targetType.Name, AsObject.GetType().Name);
        }

		public readonly object? ToBoxedValue()
		{
			return Type switch
			{
				VariantType.Int => AsInt,
				VariantType.Float => AsFloat,
				VariantType.Bool => AsBool,
				VariantType.Vector => (X, Y, Z, W),
				VariantType.Object => AsObject,
				_ => null
			};
		}

		private readonly void Ensure(VariantType expected)
        {
            if (Type != expected)
                throw new VariantCastException(expected.ToString(), Type.ToString());
        }
    }
}