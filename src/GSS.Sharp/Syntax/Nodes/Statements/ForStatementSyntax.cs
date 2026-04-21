using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class ForStatementSyntax : StatementSyntax
	{
		public SyntaxToken ForKeyword { get; }
		public StatementSyntax? Initializer { get; }
		public ExpressionSyntax? Condition { get; }
		public ExpressionSyntax? Increment { get; }
		public StatementSyntax Body { get; }
		public override SyntaxKind Kind => SyntaxKind.ForStatement;
		public override TextSpan Span => new TextSpan(ForKeyword.Position, Body.Span.End - ForKeyword.Position);

		public ForStatementSyntax(SyntaxToken forKeyword, StatementSyntax? initializer, ExpressionSyntax? condition, ExpressionSyntax? increment, StatementSyntax body)
		{
			ForKeyword = forKeyword;
			Initializer = initializer;
			Condition = condition;
			Increment = increment;
			Body = body;
		}
		public override IEnumerable<IAstNode> GetChildren()
		{
			if (Initializer != null) yield return Initializer;
			if (Condition != null) yield return Condition;
			if (Increment != null) yield return Increment;
			yield return Body;
		}
	}
}