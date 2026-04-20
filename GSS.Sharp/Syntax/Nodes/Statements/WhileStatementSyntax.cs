using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class WhileStatementSyntax : StatementSyntax
	{
		public SyntaxToken WhileKeyword { get; }
		public ExpressionSyntax Condition { get; }
		public StatementSyntax Body { get; }
		public override SyntaxKind Kind => SyntaxKind.WhileStatement;
		public override TextSpan Span => new TextSpan(WhileKeyword.Position, Body.Span.End - WhileKeyword.Position);

		public WhileStatementSyntax(SyntaxToken whileKeyword, ExpressionSyntax condition, StatementSyntax body)
		{
			WhileKeyword = whileKeyword;
			Condition = condition;
			Body = body;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Condition; yield return Body; }
	}
}