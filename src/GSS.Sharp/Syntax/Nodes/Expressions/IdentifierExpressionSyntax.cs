using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class IdentifierExpressionSyntax : ExpressionSyntax
	{
		public override bool IsAssignable => true;
		public SyntaxToken IdentifierToken { get; }
		public override SyntaxKind Kind => SyntaxKind.IdentifierExpression;
		public override TextSpan Span => new TextSpan(IdentifierToken.Position, IdentifierToken.Length);

		public IdentifierExpressionSyntax(SyntaxToken identifierToken) => IdentifierToken = identifierToken;
		public override IEnumerable<IAstNode> GetChildren() => Array.Empty<IAstNode>();
	}
}