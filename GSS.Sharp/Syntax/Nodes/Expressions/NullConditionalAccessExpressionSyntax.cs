using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class NullConditionalAccessExpressionSyntax : ExpressionSyntax
	{
		public ExpressionSyntax Expression { get; }
		public SyntaxToken QuestionDotToken { get; }
		public SyntaxToken IdentifierToken { get; }
		public override SyntaxKind Kind => SyntaxKind.NullConditionalAccessExpression;
		public override TextSpan Span => new TextSpan(Expression.Span.Start, IdentifierToken.Span.End - Expression.Span.Start);

		public NullConditionalAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken questionDotToken, SyntaxToken identifierToken)
		{
			Expression = expression;
			QuestionDotToken = questionDotToken;
			IdentifierToken = identifierToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Expression; }
	}
}