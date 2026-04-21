using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes
{
	public sealed class GenericNameSyntax : TypeSyntax
	{
		public SyntaxToken IdentifierToken { get; }
		public SyntaxToken LessToken { get; }
		public IReadOnlyList<TypeSyntax> TypeArguments { get; }
		public SyntaxToken GreaterToken { get; }
		public override SyntaxKind Kind => SyntaxKind.GenericName;
		public override TextSpan Span => new TextSpan(IdentifierToken.Position, GreaterToken.Span.End - IdentifierToken.Position);

		public GenericNameSyntax(SyntaxToken identifierToken, SyntaxToken lessToken, IReadOnlyList<TypeSyntax> typeArguments, SyntaxToken greaterToken)
		{
			IdentifierToken = identifierToken;
			LessToken = lessToken;
			TypeArguments = typeArguments;
			GreaterToken = greaterToken;
		}
		public override IEnumerable<IAstNode> GetChildren() => TypeArguments;
	}
}