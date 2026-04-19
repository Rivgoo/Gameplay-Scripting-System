using GSS.Core.ApiBinding.Models;
using GSS.Core.ApiBinding.Exceptions;

namespace GSS.Core.Runtime.Helpers
{
	public static class GssValueExtensions
	{
		public static double GetAsDouble(this in GssValue v) => v.Type switch
		{
			GssType.Double => v.AsDouble,
			GssType.Float => v.AsFloat,
			GssType.Long => v.AsLong,
			GssType.Int => v.AsInt,
			_ => throw new GssTypeMismatchException("Numeric", v.Type.ToString())
		};

		public static float GetAsFloat(this in GssValue v) => v.Type switch
		{
			GssType.Float => v.AsFloat,
			GssType.Int => v.AsInt,
			GssType.Long => v.AsLong,
			_ => throw new GssTypeMismatchException("Float/Int/Long", v.Type.ToString())
		};

		public static long GetAsLong(this in GssValue v) => v.Type switch
		{
			GssType.Long => v.AsLong,
			GssType.Int => v.AsInt,
			_ => throw new GssTypeMismatchException("Long/Int", v.Type.ToString())
		};

		public static int GetAsInt(this in GssValue v) => v.Type switch
		{
			GssType.Int => v.AsInt,
			_ => throw new GssTypeMismatchException("Int", v.Type.ToString())
		};

		public static bool IsNumeric(this in GssValue v)
		{
			return v.Type == GssType.Int || v.Type == GssType.Float ||
				   v.Type == GssType.Long || v.Type == GssType.Double;
		}

		public static string GetAsString(this in GssValue v)
		{
			if (v.Type == GssType.Null) return "null";
			if (v.Type == GssType.Object) return v.AsObject?.ToString() ?? "null";

			return v.Type switch
			{
				GssType.Int => v.AsInt.ToString(),
				GssType.Float => v.AsFloat.ToString(),
				GssType.Bool => v.AsBool.ToString(),
				GssType.Long => v.AsLong.ToString(),
				GssType.Double => v.AsDouble.ToString(),
				_ => "Unknown"
			};
		}
	}
}