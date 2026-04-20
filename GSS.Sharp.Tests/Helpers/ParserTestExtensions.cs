using GSS.Core.Compiler.Text;
using GSS.Sharp.Parsing;
using GSS.Sharp.Syntax.Nodes;
using GSS.Sharp.Syntax.Nodes.Statements;

namespace GSS.Sharp.Tests.Helpers
{
	internal static class ParserTestExtensions
	{
		public static BlockStatementSyntax ParseText(string text, out Parser parser)
		{
			var source = new SourceText(text);
			parser = new Parser(source);
			return (BlockStatementSyntax)parser.Parse();
		}

		public static ExpressionSyntax ParseSingleExpression(string text)
		{
			var ast = ParseText(text + ";", out _);
			var stmt = (ExpressionStatementSyntax)ast.Statements[0];
			return stmt.Expression;
		}
	}
}