using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class VariableDeclarationSyntax : StatementSyntax
	{
		public TypeSyntax Type { get; }
		public SyntaxToken IdentifierToken { get; }
		public SyntaxToken EqualsToken { get; }
		public ExpressionSyntax Initializer { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
		public override TextSpan Span => new TextSpan(Type.Span.Start, SemicolonToken.Span.End - Type.Span.Start);

		public VariableDeclarationSyntax(TypeSyntax type, SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax initializer, SyntaxToken semicolonToken)
		{
			Type = type;
			IdentifierToken = identifierToken;
			EqualsToken = equalsToken;
			Initializer = initializer;
			SemicolonToken = semicolonToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Type; yield return Initializer; }
	}
}