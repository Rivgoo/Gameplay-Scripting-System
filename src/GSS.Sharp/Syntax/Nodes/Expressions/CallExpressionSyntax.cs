using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Expressions
{
	public sealed class CallExpressionSyntax : ExpressionSyntax
	{
		public ExpressionSyntax Identifier { get; }
		public SyntaxToken OpenParenToken { get; }
		public IReadOnlyList<ExpressionSyntax> Arguments { get; }
		public SyntaxToken CloseParenToken { get; }
		public override SyntaxKind Kind => SyntaxKind.CallExpression;
		public override TextSpan Span => new TextSpan(Identifier.Span.Start, CloseParenToken.Position + CloseParenToken.Length - Identifier.Span.Start);

		public CallExpressionSyntax(ExpressionSyntax identifier, SyntaxToken openParenToken, IReadOnlyList<ExpressionSyntax> arguments, SyntaxToken closeParenToken)
		{
			Identifier = identifier;
			OpenParenToken = openParenToken;
			Arguments = arguments;
			CloseParenToken = closeParenToken;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Identifier; foreach (var a in Arguments) yield return a; }
	}
}