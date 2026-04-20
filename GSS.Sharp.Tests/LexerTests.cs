using FluentAssertions;
using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Text;
using GSS.Sharp.Parsing;
using GSS.Sharp.Syntax;

namespace GSS.Sharp.Tests
{
	[TestFixture]
	public class LexerTests
	{
		private List<SyntaxToken> GetTokens(string text, out Lexer lexer, bool ignoreTrivia = true)
		{
			var source = new SourceText(text);
			lexer = new Lexer(source);
			var tokens = new List<SyntaxToken>();

			while (true)
			{
				var token = lexer.NextToken();
				if (token.Kind == SyntaxKind.EndOfFileToken)
				{
					if (!ignoreTrivia) tokens.Add(token);
					break;
				}

				if (ignoreTrivia && (token.Kind == SyntaxKind.WhitespaceToken || token.Kind == SyntaxKind.CommentToken))
					continue;

				tokens.Add(token);
			}
			return tokens;
		}

		[Test]
		public void Lexer_CompoundOperators_ShouldParseAsSingleToken()
		{
			var tokens = GetTokens("== != <= >= && || ++ -- ?? .. =>", out _);

			tokens.Should().HaveCount(11);
			tokens[0].Kind.Should().Be(SyntaxKind.EqualsEqualsToken);
			tokens[8].Kind.Should().Be(SyntaxKind.QuestionQuestionToken);
			tokens[9].Kind.Should().Be(SyntaxKind.DotDotToken);
			tokens[10].Kind.Should().Be(SyntaxKind.ArrowToken);
		}

		[Test]
		public void Lexer_ValidNumbers_ShouldParseCorrectly()
		{
			var tokens = GetTokens("42 3.1415", out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeFalse();
			tokens[0].Value.Should().Be(42);
			tokens[1].Value.Should().BeOfType<float>().And.Be(3.1415f);
		}

		[Test]
		public void Lexer_InvalidNumbers_ShouldReportLex106()
		{
			GetTokens("12.34.56", out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeTrue();
			lexer.Diagnostics.Diagnostics[0].Descriptor.Code.Should().Be(DiagnosticCode.Lex106_NumericOverflow);
		}

		[Test]
		public void Lexer_KeywordsInsideIdentifiers_ShouldBeParsedAsIdentifiers()
		{
			var tokens = GetTokens("ifCondition whileLoop vartest", out _);

			tokens.Should().HaveCount(3);
			tokens.Should().AllSatisfy(t => t.Kind.Should().Be(SyntaxKind.IdentifierToken));
		}

		[Test]
		public void Lexer_UnterminatedString_ShouldReportLex101()
		{
			GetTokens("\"Broken string \n next line", out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeTrue();
			lexer.Diagnostics.Diagnostics[0].Descriptor.Code.Should().Be(DiagnosticCode.Lex101_UnterminatedString);
		}

		[Test]
		public void Lexer_UnterminatedComment_ShouldReportLex102()
		{
			GetTokens("/* Missing end", out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeTrue();
			lexer.Diagnostics.Diagnostics[0].Descriptor.Code.Should().Be(DiagnosticCode.Lex102_UnterminatedComment);
		}

		[Test]
		public void Lexer_BadCharacter_ShouldReportLex100()
		{
			var tokens = GetTokens("int a = 5 @;", out var lexer);

			tokens.Should().Contain(t => t.Kind == SyntaxKind.BadToken);
			lexer.Diagnostics.HasErrors.Should().BeTrue();
			lexer.Diagnostics.Diagnostics[0].Descriptor.Code.Should().Be(DiagnosticCode.Lex100_UnexpectedCharacter);
		}

		[Test]
		public void Lexer_InterpolatedString_ShouldSwitchModesAndParseTokens()
		{
			var tokens = GetTokens("$\"Player {name} lvl {level}\"", out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeFalse();
			tokens[0].Kind.Should().Be(SyntaxKind.InterpolatedStringStartToken);
			tokens[1].Kind.Should().Be(SyntaxKind.InterpolatedStringTextToken);
			tokens[2].Kind.Should().Be(SyntaxKind.OpenBraceToken);
			tokens[3].Kind.Should().Be(SyntaxKind.IdentifierToken);
			tokens[4].Kind.Should().Be(SyntaxKind.CloseBraceToken);
			tokens[5].Kind.Should().Be(SyntaxKind.InterpolatedStringTextToken);
			tokens[9].Kind.Should().Be(SyntaxKind.InterpolatedStringEndToken);
		}

		[Test]
		public void Lexer_Trivia_BindsCommentsAndWhitespaceCorrectly()
		{
			string text = "/* doc */\nvar x = 1; // end";
			var tokens = GetTokens(text, out _, ignoreTrivia: false);

			var varToken = tokens[0];
			varToken.Kind.Should().Be(SyntaxKind.VarKeyword);
			varToken.LeadingTrivia.Should().HaveCount(2);
			varToken.LeadingTrivia[0].Kind.Should().Be(SyntaxKind.CommentToken);
			varToken.LeadingTrivia[1].Kind.Should().Be(SyntaxKind.WhitespaceToken);

			var semicolonToken = tokens[4];
			semicolonToken.Kind.Should().Be(SyntaxKind.SemicolonToken);
			semicolonToken.TrailingTrivia.Should().HaveCount(2);
			semicolonToken.TrailingTrivia[0].Kind.Should().Be(SyntaxKind.WhitespaceToken);
			semicolonToken.TrailingTrivia[1].Kind.Should().Be(SyntaxKind.CommentToken);
		}

		[Test]
		public void Lexer_EndOfFile_CapturesLeadingTrivia()
		{
			string text = "// final note";
			var tokens = GetTokens(text, out _, ignoreTrivia: false);

			var eofToken = tokens[^1];
			eofToken.Kind.Should().Be(SyntaxKind.EndOfFileToken);
			eofToken.LeadingTrivia.Should().Contain(t => t.Kind == SyntaxKind.CommentToken);
		}

		[Test]
		public void Lexer_StringEscapes_RecoversFromInvalidSequences()
		{
			string text = "\"Valid \\n Invalid \\q \"";
			GetTokens(text, out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeTrue();
			lexer.Diagnostics.Diagnostics.Should().ContainSingle(d => d.Descriptor.Code == DiagnosticCode.Lex105_InvalidEscapeSequence);
		}

		[Test]
		public void Lexer_NumericOverflow_PreventsCrashAndReportsDiagnostic()
		{
			string text = "999999999999999999999999999";
			var tokens = GetTokens(text, out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeTrue();
			lexer.Diagnostics.Diagnostics.Should().ContainSingle(d => d.Descriptor.Code == DiagnosticCode.Lex106_NumericOverflow);
			tokens[0].Value.Should().Be(0);
		}

		[Test]
		public void Lexer_InterpolatedString_TracksNestedBraceDepth()
		{
			string text = "$\"Value { obj.Calculate({x, y}) }\"";
			var tokens = GetTokens(text, out var lexer);

			lexer.Diagnostics.HasErrors.Should().BeFalse();
			tokens.Should().Contain(t => t.Kind == SyntaxKind.InterpolatedStringTextToken);
			tokens.Should().Contain(t => t.Kind == SyntaxKind.IdentifierToken && t.GetText() == "obj");
		}
	}
}