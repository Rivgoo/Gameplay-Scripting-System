using FluentAssertions;
using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Exceptions;
using GSS.Sharp.Syntax.Nodes.Statements;
using GSS.Sharp.Tests.Helpers;
using System.Text;

namespace GSS.Sharp.Tests
{
	[TestFixture]
	public class ParserResilienceTests
	{
		[Test]
		public void Parser_PanicMode_ShouldRecoverFromGarbageInStatement()
		{
			var ast = ParserTestExtensions.ParseText("int a = 5 %*@; var b = 10;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();

			var validStmt = ast.Statements[^1].Should().BeOfType<ImplicitVariableDeclarationSyntax>().Subject;
			validStmt.IdentifierToken.Text.Should().Be("b");
		}

		[Test]
		public void Parser_MissingSemicolon_ShouldReportAndRecover()
		{
			var ast = ParserTestExtensions.ParseText("int a = 5 \n int b = 10;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().Contain(d => d.Descriptor.Code == DiagnosticCode.Syn204_MissingToken);

			ast.Statements.Count.Should().Be(2);
		}

		[Test]
		public void Parser_StackOverflowGuard_ShouldThrowCompilationCanceledException()
		{
			var sb = new StringBuilder();
			sb.Append('(', 600);
			sb.Append('1');
			sb.Append(')', 600);
			sb.Append(';');

			Action act = () => ParserTestExtensions.ParseText(sb.ToString(), out _);

			act.Should().Throw<GssCompilationCanceledException>()
			   .WithMessage("*recursion depth exceeded*");
		}

		[Test]
		public void Parser_Resilience_SynthesizesMissingControlFlowTokens()
		{
			var ast = ParserTestExtensions.ParseText("if (health > 0 { Die(); }", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().Contain(d => d.Descriptor.Code == DiagnosticCode.Syn204_MissingToken);

			var ifStmt = ast.Statements[0].Should().BeOfType<IfStatementSyntax>().Subject;
			ifStmt.ThenStatement.Should().BeOfType<BlockStatementSyntax>();
		}

		[Test]
		public void Parser_Resilience_SkipsGarbageInsideBlocks()
		{
			var ast = ParserTestExtensions.ParseText("{ int a = 1; &*^%#; int b = 2; }", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().Contain(d => d.Descriptor.Code == DiagnosticCode.Syn206_SkippedInvalidTokens);

			var block = ast.Statements[0].Should().BeOfType<BlockStatementSyntax>().Subject;
			block.Statements.Should().HaveCount(3);
			block.Statements[1].Should().BeOfType<ExpressionStatementSyntax>();
		}

		[Test]
		public void Parser_Resilience_SafelyHaltsOnUnexpectedEndOfFile()
		{
			ParserTestExtensions.ParseText("var x = 5 + ", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().Contain(d => d.Descriptor.Code == DiagnosticCode.Syn201_ExpectedExpression);
		}

		[Test]
		public void Parser_Resilience_HandlesSemicolonStorms()
		{
			var ast = ParserTestExtensions.ParseText(";;;;;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().Contain(d => d.Descriptor.Code == DiagnosticCode.Syn201_ExpectedExpression);
		}
	}
}