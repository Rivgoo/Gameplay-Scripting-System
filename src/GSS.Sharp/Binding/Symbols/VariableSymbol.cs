namespace GSS.Sharp.Binding.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        public TypeSymbol Type { get; }
        public int RegisterIndex { get; }
        public bool IsReadOnly { get; }
        public bool IsAssigned { get; set; }

        public VariableSymbol(string name, TypeSymbol type, int registerIndex, bool isReadOnly, bool isAssigned)
            : base(name)
        {
            Type = type;
            RegisterIndex = registerIndex;
            IsReadOnly = isReadOnly;
            IsAssigned = isAssigned;
        }
    }
}