namespace GSS.Sharp.Binding.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new("?");
        public static readonly TypeSymbol Void = new("void");
        public static readonly TypeSymbol Any = new("any");
        public static readonly TypeSymbol Bool = new("bool");
        public static readonly TypeSymbol Int = new("int");
        public static readonly TypeSymbol Float = new("float");
        public static readonly TypeSymbol String = new("string");
        public static readonly TypeSymbol Vector = new("vector");

        private TypeSymbol(string name) : base(name) { }

        public override string ToString() => Name;
    }
}