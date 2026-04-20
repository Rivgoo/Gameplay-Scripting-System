using FluentAssertions;
using GSS.Sharp.Syntax;
using GSS.Sharp.Syntax.Nodes.Statements;
using GSS.Sharp.Tests.Helpers;

namespace GSS.Sharp.Tests
{
	[TestFixture]
	public class ParserStatementTests
	{
		[Test]
		public void Parser_VariableDeclarations_ShouldParseExplicitAndImplicit()
		{
			var ast = ParserTestExtensions.ParseText("int a = 5; var b = 10;", out _);

			ast.Statements[0].Should().BeOfType<VariableDeclarationSyntax>();
			ast.Statements[1].Should().BeOfType<ImplicitVariableDeclarationSyntax>();
		}

		[Test]
		public void Parser_DanglingElse_ShouldAttachToInnerIf()
		{
			var ast = ParserTestExtensions.ParseText("if (a) if (b) foo(); else bar();", out _);

			var outerIf = ast.Statements[0].Should().BeOfType<IfStatementSyntax>().Subject;
			outerIf.ElseStatement.Should().BeNull();

			var innerIf = outerIf.ThenStatement.Should().BeOfType<IfStatementSyntax>().Subject;
			innerIf.ElseStatement.Should().NotBeNull();
		}

		[Test]
		public void Parser_ForEachLoop_ShouldParseSugarSyntax()
		{
			var ast = ParserTestExtensions.ParseText("for (var i in 0..10) { }", out _);

			var forEach = ast.Statements[0].Should().BeOfType<ForEachStatementSyntax>().Subject;
			forEach.IdentifierToken.Text.Should().Be("i");
			forEach.CollectionExpression.Kind.Should().Be(SyntaxKind.RangeExpression);
		}

		[Test]
		public void Parser_EdgeCases_HandlesCommentsOnlyFile()
		{
			var ast = ParserTestExtensions.ParseText("/* purely comments */ \n // nothing else", out var parser);

			parser.Diagnostics.HasErrors.Should().BeFalse();
			ast.Statements.Should().BeEmpty();
		}

		[Test]
		public void Parser_EdgeCases_HandlesEmptyReturn()
		{
			var ast = ParserTestExtensions.ParseText("return;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeFalse();
			var ret = ast.Statements[0].Should().BeOfType<ReturnStatementSyntax>().Subject;
			ret.Expression.Should().BeNull();
		}

		[Test]
		public void Parser_ImportDirectives_ShouldParseSuccessfully()
		{
			var ast = ParserTestExtensions.ParseText("@import DoorScript; var a = 1;", out var parser);

			ast = ParserTestExtensions.ParseText("import DoorScript; var a = 1;", out parser);

			parser.Diagnostics.HasErrors.Should().BeFalse();
			var import = ast.Statements[0].Should().BeOfType<ImportDirectiveSyntax>().Subject;
			import.IdentifierToken.GetText().Should().Be("DoorScript");
		}
	}
}