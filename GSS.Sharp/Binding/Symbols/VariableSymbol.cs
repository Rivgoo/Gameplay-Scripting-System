namespace GSS.Sharp.Binding.Symbols
{
	public sealed class VariableSymbol : Symbol
	{
		public TypeSymbol Type { get; }
		public int RegisterIndex { get; }
		public bool IsReadOnly { get; }

		public VariableSymbol(string name, TypeSymbol type, int registerIndex, bool isReadOnly)
			: base(name)
		{
			Type = type;
			RegisterIndex = registerIndex;
			IsReadOnly = isReadOnly;
		}
	}
}