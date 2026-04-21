using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ReturnStatementSyntax : StatementSyntax
	{
		public SyntaxToken ReturnKeyword { get; }
		public ExpressionSyntax? Expression { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
		public override TextSpan Span => new TextSpan(ReturnKeyword.Position, SemicolonToken.Position + SemicolonToken.Length - ReturnKeyword.Position);

		public ReturnStatementSyntax(SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
		{
			ReturnKeyword = returnKeyword;
			Expression = expression;
			SemicolonToken = semicolonToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { if (Expression != null) yield return Expression; }
	}
}