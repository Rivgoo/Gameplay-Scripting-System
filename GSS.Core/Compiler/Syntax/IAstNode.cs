namespace GSS.Core.Compiler.Syntax
{
	public interface IAstNode
	{
		int Kind { get; }
		IEnumerable<IAstNode> GetChildren();
	}
}
