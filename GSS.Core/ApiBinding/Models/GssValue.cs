using System.Runtime.InteropServices;
using GSS.Core.ApiBinding.Exceptions;

namespace GSS.Core.ApiBinding.Models
{
	[StructLayout(LayoutKind.Explicit, Size = 24)]
	public struct GssValue
	{
		[FieldOffset(0)] public GssType Type;

		[FieldOffset(4)] public int AsInt;
		[FieldOffset(4)] public float AsFloat;
		[FieldOffset(4)] public bool AsBool;
		[FieldOffset(4)] public long AsLong;
		[FieldOffset(4)] public double AsDouble;

		[FieldOffset(4)] public float X;
		[FieldOffset(8)] public float Y;
		[FieldOffset(12)] public float Z;

		[FieldOffset(16)] public object? AsObject;

		public static GssValue Null => new() { Type = GssType.Null };
		public static GssValue FromInt(int v) => new() { Type = GssType.Int, AsInt = v };
		public static GssValue FromFloat(float v) => new() { Type = GssType.Float, AsFloat = v };
		public static GssValue FromBool(bool v) => new() { Type = GssType.Bool, AsBool = v };
		public static GssValue FromDouble(double v) => new() { Type = GssType.Double, AsDouble = v };
		public static GssValue FromLong(long v) => new() { Type = GssType.Long, AsLong = v };
		public static GssValue FromObject(object? v) => new() { Type = GssType.Object, AsObject = v };

		public static GssValue FromVector3(float x, float y, float z) => new() { Type = GssType.Object, X = x, Y = y, Z = z };

		public readonly T Unbox<T>()
		{
			Type targetType = typeof(T);

			if (targetType == typeof(float)) { Ensure(GssType.Float); return (T)(object)AsFloat; }
			if (targetType == typeof(int)) { Ensure(GssType.Int); return (T)(object)AsInt; }
			if (targetType == typeof(bool)) { Ensure(GssType.Bool); return (T)(object)AsBool; }
			if (targetType == typeof(double)) { Ensure(GssType.Double); return (T)(object)AsDouble; }
			if (targetType == typeof(long)) { Ensure(GssType.Long); return (T)(object)AsLong; }

			Ensure(GssType.Object);

			if (AsObject == null)
			{
				if (default(T) == null) return default!;
				throw new GssValidationException($"Cannot convert null to non-nullable value type {targetType.Name}.");
			}

			if (AsObject is T typedObject) return typedObject;

			throw new GssTypeMismatchException(targetType.Name, AsObject.GetType().Name);
		}

		private readonly void Ensure(GssType expected)
		{
			if (Type != expected)
				throw new GssTypeMismatchException(expected.ToString(), Type.ToString());
		}

		public readonly object? ToBoxedValue()
		{
			return Type switch
			{
				GssType.Int => AsInt,
				GssType.Float => AsFloat,
				GssType.Bool => AsBool,
				GssType.Long => AsLong,
				GssType.Double => AsDouble,
				GssType.Object => AsObject,
				_ => null
			};
		}
	}
}