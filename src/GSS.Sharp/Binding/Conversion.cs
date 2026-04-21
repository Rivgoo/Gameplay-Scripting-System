using GSS.Sharp.Binding.Symbols;

namespace GSS.Sharp.Binding
{
    public sealed class Conversion
    {
        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists; IsIdentity = isIdentity; IsImplicit = isImplicit;
        }

        public static readonly Conversion None = new(false, false, false);
        public static readonly Conversion Identity = new(true, true, true);
        public static readonly Conversion Implicit = new(true, false, true);

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to) return Identity;

            if (to == TypeSymbol.String) return Implicit;

            if (to == TypeSymbol.Any || from == TypeSymbol.Any) return Implicit;

            if (from == TypeSymbol.Int && to == TypeSymbol.Float) return Implicit;

            return None;
        }
    }
}