using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class WaitStatementSyntax : StatementSyntax
	{
		public SyntaxToken WaitKeyword { get; }
		public ExpressionSyntax Duration { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.WaitStatement;
		public override TextSpan Span => new TextSpan(WaitKeyword.Position, SemicolonToken.Span.End - WaitKeyword.Position);

		public WaitStatementSyntax(SyntaxToken waitKeyword, ExpressionSyntax duration, SyntaxToken semicolonToken)
		{
			WaitKeyword = waitKeyword;
			Duration = duration;
			SemicolonToken = semicolonToken;
		}

		public override IEnumerable<IAstNode> GetChildren() { yield return Duration; }
	}
}