using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ImportDirectiveSyntax : StatementSyntax
	{
		public SyntaxToken ImportKeyword { get; }
		public SyntaxToken IdentifierToken { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.ImportDirective;
		public override TextSpan Span => new TextSpan(ImportKeyword.Position, SemicolonToken.Position + SemicolonToken.Length - ImportKeyword.Position);

		public ImportDirectiveSyntax(SyntaxToken importKeyword, SyntaxToken identifierToken, SyntaxToken semicolonToken)
		{
			ImportKeyword = importKeyword;
			IdentifierToken = identifierToken;
			SemicolonToken = semicolonToken;
		}

		public override IEnumerable<IAstNode> GetChildren() => Array.Empty<IAstNode>();
	}
}