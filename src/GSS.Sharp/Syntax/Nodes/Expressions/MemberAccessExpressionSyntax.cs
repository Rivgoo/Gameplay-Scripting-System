using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
	{
		public override bool IsAssignable => true;
		public ExpressionSyntax Expression { get; }
		public SyntaxToken DotToken { get; }
		public SyntaxToken IdentifierToken { get; }
		public override SyntaxKind Kind => SyntaxKind.MemberAccessExpression;
		public override TextSpan Span => new TextSpan(Expression.Span.Start, IdentifierToken.Position + IdentifierToken.Length - Expression.Span.Start);

		public MemberAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken dotToken, SyntaxToken identifierToken)
		{
			Expression = expression;
			DotToken = dotToken;
			IdentifierToken = identifierToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Expression; }
	}
}