namespace GSS.Sharp.Binding.Nodes
{
	public abstract class BoundExpression : BoundNode
	{
		public abstract Symbols.TypeSymbol Type { get; }
	}
}