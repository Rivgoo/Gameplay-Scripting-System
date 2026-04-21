using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class PostfixUnaryExpressionSyntax : ExpressionSyntax
	{
		public ExpressionSyntax Operand { get; }
		public SyntaxToken OperatorToken { get; }
		public override SyntaxKind Kind => SyntaxKind.PostfixUnaryExpression;
		public override TextSpan Span => new TextSpan(Operand.Span.Start, OperatorToken.Span.End - Operand.Span.Start);

		public PostfixUnaryExpressionSyntax(ExpressionSyntax operand, SyntaxToken operatorToken)
		{
			Operand = operand;
			OperatorToken = operatorToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Operand; }
	}
}