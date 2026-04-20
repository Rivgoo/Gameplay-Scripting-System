using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class BreakStatementSyntax : StatementSyntax
	{
		public SyntaxToken BreakKeyword { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.BreakStatement;
		public override TextSpan Span => new TextSpan(BreakKeyword.Position, SemicolonToken.Position + SemicolonToken.Length - BreakKeyword.Position);

		public BreakStatementSyntax(SyntaxToken breakKeyword, SyntaxToken semicolonToken)
		{
			BreakKeyword = breakKeyword;
			SemicolonToken = semicolonToken;
		}
		public override IEnumerable<IAstNode> GetChildren() => Array.Empty<IAstNode>();
	}
}