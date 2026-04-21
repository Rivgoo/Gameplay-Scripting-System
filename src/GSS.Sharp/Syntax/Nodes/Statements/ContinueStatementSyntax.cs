using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ContinueStatementSyntax : StatementSyntax
	{
		public SyntaxToken ContinueKeyword { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.ContinueStatement;
		public override TextSpan Span => new TextSpan(ContinueKeyword.Position, SemicolonToken.Position + SemicolonToken.Length - ContinueKeyword.Position);

		public ContinueStatementSyntax(SyntaxToken continueKeyword, SyntaxToken semicolonToken)
		{
			ContinueKeyword = continueKeyword;
			SemicolonToken = semicolonToken;
		}
		public override IEnumerable<IAstNode> GetChildren() => Array.Empty<IAstNode>();
	}
}