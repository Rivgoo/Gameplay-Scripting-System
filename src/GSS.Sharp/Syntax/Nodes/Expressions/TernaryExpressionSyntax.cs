using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class TernaryExpressionSyntax : ExpressionSyntax
	{
		public ExpressionSyntax Condition { get; }
		public SyntaxToken QuestionToken { get; }
		public ExpressionSyntax TrueExpression { get; }
		public SyntaxToken ColonToken { get; }
		public ExpressionSyntax FalseExpression { get; }
		public override SyntaxKind Kind => SyntaxKind.TernaryExpression;
		public override TextSpan Span => new TextSpan(Condition.Span.Start, FalseExpression.Span.End - Condition.Span.Start);

		public TernaryExpressionSyntax(ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax trueExpr, SyntaxToken colonToken, ExpressionSyntax falseExpr)
		{
			Condition = condition;
			QuestionToken = questionToken;
			TrueExpression = trueExpr;
			ColonToken = colonToken;
			FalseExpression = falseExpr;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Condition; yield return TrueExpression; yield return FalseExpression; }
	}
}