using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Types
{
	public sealed class IdentifierNameSyntax : TypeSyntax
	{
		public SyntaxToken IdentifierToken { get; }
		public override SyntaxKind Kind => SyntaxKind.IdentifierName;
		public override TextSpan Span => IdentifierToken.Span;

		public IdentifierNameSyntax(SyntaxToken identifierToken) => IdentifierToken = identifierToken;
		public override IEnumerable<IAstNode> GetChildren() => Array.Empty<IAstNode>();
	}
}