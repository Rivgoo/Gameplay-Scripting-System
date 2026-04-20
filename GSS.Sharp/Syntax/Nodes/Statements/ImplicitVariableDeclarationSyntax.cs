using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ImplicitVariableDeclarationSyntax : StatementSyntax
	{
		public SyntaxToken VarKeyword { get; }
		public SyntaxToken IdentifierToken { get; }
		public SyntaxToken EqualsToken { get; }
		public ExpressionSyntax Initializer { get; }
		public SyntaxToken SemicolonToken { get; }
		public override SyntaxKind Kind => SyntaxKind.ImplicitVariableDeclaration;
		public override TextSpan Span => new TextSpan(VarKeyword.Position, SemicolonToken.Position + SemicolonToken.Length - VarKeyword.Position);

		public ImplicitVariableDeclarationSyntax(SyntaxToken varKeyword, SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax initializer, SyntaxToken semicolonToken)
		{
			VarKeyword = varKeyword;
			IdentifierToken = identifierToken;
			EqualsToken = equalsToken;
			Initializer = initializer;
			SemicolonToken = semicolonToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Initializer; }
	}
}