using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class BlockStatementSyntax : StatementSyntax
	{
		public SyntaxToken OpenBraceToken { get; }
		public IReadOnlyList<StatementSyntax> Statements { get; }
		public SyntaxToken CloseBraceToken { get; }
		public override SyntaxKind Kind => SyntaxKind.BlockStatement;
		public override TextSpan Span => new TextSpan(OpenBraceToken.Position, CloseBraceToken.Position + CloseBraceToken.Length - OpenBraceToken.Position);

		public BlockStatementSyntax(SyntaxToken openBraceToken, IReadOnlyList<StatementSyntax> statements, SyntaxToken closeBraceToken)
		{
			OpenBraceToken = openBraceToken;
			Statements = statements;
			CloseBraceToken = closeBraceToken;
		}
		public override IEnumerable<IAstNode> GetChildren() => Statements;
	}
}