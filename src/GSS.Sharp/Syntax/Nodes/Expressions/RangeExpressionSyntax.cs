using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class RangeExpressionSyntax : ExpressionSyntax
	{
		public ExpressionSyntax StartExpression { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax EndExpression { get; }
		public override SyntaxKind Kind => SyntaxKind.RangeExpression;
		public override TextSpan Span => new TextSpan(StartExpression.Span.Start, EndExpression.Span.End - StartExpression.Span.Start);

		public RangeExpressionSyntax(ExpressionSyntax startExpr, SyntaxToken operatorToken, ExpressionSyntax endExpr)
		{
			StartExpression = startExpr;
			OperatorToken = operatorToken;
			EndExpression = endExpr;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return StartExpression; yield return EndExpression; }
	}
}