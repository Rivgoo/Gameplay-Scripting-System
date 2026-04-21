using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class ElementAccessExpressionSyntax : ExpressionSyntax
	{
		public override bool IsAssignable => true;
		public ExpressionSyntax Expression { get; }
		public SyntaxToken OpenBracketToken { get; }
		public ExpressionSyntax IndexArgument { get; }
		public SyntaxToken CloseBracketToken { get; }
		public override SyntaxKind Kind => SyntaxKind.ElementAccessExpression;
		public override TextSpan Span => new TextSpan(Expression.Span.Start, CloseBracketToken.Span.End - Expression.Span.Start);

		public ElementAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken openBracketToken, ExpressionSyntax indexArgument, SyntaxToken closeBracketToken)
		{
			Expression = expression;
			OpenBracketToken = openBracketToken;
			IndexArgument = indexArgument;
			CloseBracketToken = closeBracketToken;
		}

		public override IEnumerable<IAstNode> GetChildren() { yield return Expression; yield return IndexArgument; }
	}
}