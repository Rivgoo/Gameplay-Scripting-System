using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.ApiBinding.Models;

namespace GSS.Core.Runtime.Helpers
{
	public static class GssValuePacker
	{
		public static GssValue Pack(object? obj)
		{
			if (obj == null) return GssValue.Null;
			if (obj is int i) return GssValue.FromInt(i);
			if (obj is float f) return GssValue.FromFloat(f);
			if (obj is bool b) return GssValue.FromBool(b);
			if (obj is double d) return GssValue.FromDouble(d);
			if (obj is long l) return GssValue.FromLong(l);
			return GssValue.FromObject(obj);
		}

		public static GssType GetGssType(Type type)
		{
			if (type == typeof(int)) return GssType.Int;
			if (type == typeof(float)) return GssType.Float;
			if (type == typeof(bool)) return GssType.Bool;
			if (type == typeof(long)) return GssType.Long;
			if (type == typeof(double)) return GssType.Double;
			if (type == typeof(void) || type == typeof(VoidResult)) return GssType.Null;
			return GssType.Object;
		}
	}
}