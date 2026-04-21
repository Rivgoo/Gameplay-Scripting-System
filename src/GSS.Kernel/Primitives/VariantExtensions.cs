namespace GSS.Kernel.Primitives
{
    public static class VariantExtensions
    {
        public static Variant Pack(object? obj)
        {
            if (obj == null) return Variant.Null;
            if (obj is int i) return Variant.FromInt(i);
            if (obj is float f) return Variant.FromFloat(f);
            if (obj is bool b) return Variant.FromBool(b);
            
            return Variant.FromObject(obj);
        }

        public static VariantType GetVariantType(Type type)
        {
            if (type == typeof(int)) return VariantType.Int;
            if (type == typeof(float)) return VariantType.Float;
            if (type == typeof(bool)) return VariantType.Bool;
            if (type == typeof(void)) return VariantType.Null;
            return VariantType.Object;
        }

        public static string GetAsString(this in Variant v)
        {
            if (v.Type == VariantType.Null) return "null";
            if (v.Type == VariantType.Object) return v.AsObject?.ToString() ?? "null";

            return v.Type switch
            {
                VariantType.Int => v.AsInt.ToString(),
                VariantType.Float => v.AsFloat.ToString(),
                VariantType.Bool => v.AsBool.ToString(),
                VariantType.Vector => $"({v.X}, {v.Y}, {v.Z}, {v.W})",
                _ => "Unknown"
            };
        }
    }
}