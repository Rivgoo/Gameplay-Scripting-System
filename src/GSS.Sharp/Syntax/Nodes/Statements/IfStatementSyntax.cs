using GSS.Kernel.Diagnostics;
using GSS.Kernel.Compiler.Syntax;

namespace GSS.Sharp.Syntax.Nodes.Statements
{
	public sealed class IfStatementSyntax : StatementSyntax
	{
		public SyntaxToken IfKeyword { get; }
		public ExpressionSyntax Condition { get; }
		public StatementSyntax ThenStatement { get; }
		public StatementSyntax? ElseStatement { get; }
		public override SyntaxKind Kind => SyntaxKind.IfStatement;
		public override TextSpan Span => new TextSpan(IfKeyword.Position, (ElseStatement?.Span.End ?? ThenStatement.Span.End) - IfKeyword.Position);

		public IfStatementSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, StatementSyntax thenStatement, StatementSyntax? elseStatement)
		{
			IfKeyword = ifKeyword;
			Condition = condition;
			ThenStatement = thenStatement;
			ElseStatement = elseStatement;
		}
		public override IEnumerable<IAstNode> GetChildren() { yield return Condition; yield return ThenStatement; if (ElseStatement != null) yield return ElseStatement; }
	}
}