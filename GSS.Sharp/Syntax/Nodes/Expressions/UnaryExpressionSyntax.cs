using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class UnaryExpressionSyntax : ExpressionSyntax
	{
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Operand { get; }
		public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
		public override TextSpan Span => new TextSpan(OperatorToken.Position, Operand.Span.End - OperatorToken.Position);

		public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			OperatorToken = operatorToken;
			Operand = operand;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Operand; }
	}
}