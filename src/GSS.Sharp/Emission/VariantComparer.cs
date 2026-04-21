using GSS.Kernel.Primitives;

namespace GSS.Sharp.Emission
{
    public readonly struct VariantComparer : IEqualityComparer<Variant>
    {
        public bool Equals(Variant x, Variant y)
        {
            if (x.Type != y.Type) return false;
            return x.Type switch
            {
                VariantType.Int => x.AsInt == y.AsInt,
                VariantType.Float => x.AsFloat == y.AsFloat,
                VariantType.Bool => x.AsBool == y.AsBool,
                VariantType.Null => true,
                VariantType.Vector => x.X == y.X && x.Y == y.Y && x.Z == y.Z && x.W == y.W,
                _ => ReferenceEquals(x.AsObject, y.AsObject)
            };
        }

        public int GetHashCode(Variant obj)
        {
            return obj.Type switch
            {
                VariantType.Int => HashCode.Combine(obj.Type, obj.AsInt),
                VariantType.Float => HashCode.Combine(obj.Type, obj.AsFloat),
                VariantType.Bool => HashCode.Combine(obj.Type, obj.AsBool),
                VariantType.Vector => HashCode.Combine(obj.Type, obj.X, obj.Y, obj.Z, obj.W),
                VariantType.Null => HashCode.Combine(obj.Type, 0),
                _ => HashCode.Combine(obj.Type, obj.AsObject?.GetHashCode() ?? 0)
            };
        }
    }
}