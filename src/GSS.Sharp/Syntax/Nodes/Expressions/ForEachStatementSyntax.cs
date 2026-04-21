using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ForEachStatementSyntax : StatementSyntax, IScopeDefiningNode
	{
		public SyntaxToken ForKeyword { get; }
		public SyntaxToken OpenParenToken { get; }
		public SyntaxToken VarKeyword { get; }
		public SyntaxToken IdentifierToken { get; }
		public SyntaxToken InKeyword { get; }
		public ExpressionSyntax CollectionExpression { get; }
		public SyntaxToken CloseParenToken { get; }
		public StatementSyntax Body { get; }
		public override SyntaxKind Kind => SyntaxKind.ForEachStatement;
		public override TextSpan Span => new TextSpan(ForKeyword.Position, Body.Span.End - ForKeyword.Position);

		public ForEachStatementSyntax(
			SyntaxToken forKeyword,
			SyntaxToken openParenToken,
			SyntaxToken varKeyword,
			SyntaxToken identifier,
			SyntaxToken inKeyword,
			ExpressionSyntax collection,
			SyntaxToken closeParenToken,
			StatementSyntax body)
		{
			ForKeyword = forKeyword;
			OpenParenToken = openParenToken;
			VarKeyword = varKeyword;
			IdentifierToken = identifier;
			InKeyword = inKeyword;
			CollectionExpression = collection;
			CloseParenToken = closeParenToken;
			Body = body;
		}

		public override IEnumerable<IAstNode> GetChildren()
		{
			yield return CollectionExpression;
			yield return Body;
		}
	}
}