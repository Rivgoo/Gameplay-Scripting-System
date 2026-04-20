using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ExpressionStatementSyntax : StatementSyntax
	{
		public ExpressionSyntax Expression { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
		public override TextSpan Span => new TextSpan(Expression.Span.Start, SemicolonToken.Position + SemicolonToken.Length - Expression.Span.Start);

		public ExpressionStatementSyntax(ExpressionSyntax expression, SyntaxToken semicolonToken)
		{
			Expression = expression;
			SemicolonToken = semicolonToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Expression; }
	}
}