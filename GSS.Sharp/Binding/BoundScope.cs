using GSS.Sharp.Binding.Symbols;

namespace GSS.Sharp.Binding
{
	public sealed class BoundScope
	{
		public BoundScope? Parent { get; }
		public int VariableOffset { get; }
		private readonly Dictionary<string, VariableSymbol> _variables = new();

		public BoundScope(BoundScope? parent, int variableOffset)
		{
			Parent = parent;
			VariableOffset = variableOffset;
		}

		public bool TryDeclare(VariableSymbol variable)
		{
			if (_variables.ContainsKey(variable.Name)) return false;
			_variables.Add(variable.Name, variable);
			return true;
		}

		public bool TryLookup(string name, out VariableSymbol variable)
		{
			if (_variables.TryGetValue(name, out var symbol))
			{
				variable = symbol;
				return true;
			}

			if (Parent != null) return Parent.TryLookup(name, out variable);

			variable = null!;
			return false;
		}

		public int GetAllocatedVariableCount()
		{
			return VariableOffset + _variables.Count;
		}
	}
}