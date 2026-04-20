using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class BinaryExpressionSyntax : ExpressionSyntax
	{
		public ExpressionSyntax Left { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Right { get; }
		public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
		public override TextSpan Span => new TextSpan(Left.Span.Start, Right.Span.End - Left.Span.Start);

		public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			Left = left;
			OperatorToken = operatorToken;
			Right = right;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Left; yield return Right; }
	}
}