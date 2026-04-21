using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class LiteralExpressionSyntax : ExpressionSyntax
	{
		public SyntaxToken LiteralToken { get; }
		public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
		public override TextSpan Span => new TextSpan(LiteralToken.Position, LiteralToken.Length);

		public LiteralExpressionSyntax(SyntaxToken literalToken) => LiteralToken = literalToken;
		public override IEnumerable<IAstNode> GetChildren() => Array.Empty<IAstNode>();
	}
}