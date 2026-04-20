namespace GSS.Sharp.Syntax.Nodes
{
	public abstract class ExpressionSyntax : AstNode
	{
		public virtual bool IsAssignable => false;
	}
}