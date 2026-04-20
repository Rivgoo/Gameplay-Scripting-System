using GSS.Core.ApiBinding.Models;

namespace GSS.Sharp.Emission
{
	public readonly struct GssValueComparer : IEqualityComparer<GssValue>
	{
		public bool Equals(GssValue x, GssValue y)
		{
			if (x.Type != y.Type) return false;
			return x.Type switch
			{
				GssType.Int => x.AsInt == y.AsInt,
				GssType.Float => x.AsFloat == y.AsFloat,
				GssType.Bool => x.AsBool == y.AsBool,
				GssType.Long => x.AsLong == y.AsLong,
				GssType.Double => x.AsDouble == y.AsDouble,
				GssType.Null => true,
				_ => ReferenceEquals(x.AsObject, y.AsObject)
			};
		}

		public int GetHashCode(GssValue obj)
		{
			return obj.Type switch
			{
				GssType.Int => HashCode.Combine(obj.Type, obj.AsInt),
				GssType.Float => HashCode.Combine(obj.Type, obj.AsFloat),
				GssType.Bool => HashCode.Combine(obj.Type, obj.AsBool),
				GssType.Long => HashCode.Combine(obj.Type, obj.AsLong),
				GssType.Double => HashCode.Combine(obj.Type, obj.AsDouble),
				GssType.Null => HashCode.Combine(obj.Type, 0),
				_ => HashCode.Combine(obj.Type, obj.AsObject?.GetHashCode() ?? 0)
			};
		}
	}
}