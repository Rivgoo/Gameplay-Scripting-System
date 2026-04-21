using FluentAssertions;
using GSS.Sharp.Syntax;
using GSS.Sharp.Syntax.Nodes.Expressions;
using GSS.Sharp.Syntax.Nodes.Statements;
using GSS.Sharp.Tests.Helpers;

namespace GSS.Sharp.Tests
{
	[TestFixture]
	public class ParserExpressionTests
	{
		[Test]
		public void Parser_MathPrecedence_ShouldGroupMultiplyBeforeAdd()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("1 + 2 * 3");

			var binary = expr.Should().BeOfType<BinaryExpressionSyntax>().Subject;
			binary.OperatorToken.Kind.Should().Be(SyntaxKind.PlusToken);
			binary.Right.Should().BeOfType<BinaryExpressionSyntax>()
				.Which.OperatorToken.Kind.Should().Be(SyntaxKind.StarToken);
		}

		[Test]
		public void Parser_LeftAssociativity_ShouldGroupLeftToRight()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("10 - 5 - 2");

			var binary = expr.Should().BeOfType<BinaryExpressionSyntax>().Subject;
			binary.Right.Should().BeOfType<LiteralExpressionSyntax>();
			binary.Left.Should().BeOfType<BinaryExpressionSyntax>();
		}

		[Test]
		public void Parser_RightAssociativity_ShouldGroupRightToLeftForAssignments()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("a = b = 5");

			var assign1 = expr.Should().BeOfType<AssignmentExpressionSyntax>().Subject;
			assign1.Left.Should().BeOfType<IdentifierExpressionSyntax>();

			var assign2 = assign1.Right.Should().BeOfType<AssignmentExpressionSyntax>().Subject;
			assign2.Left.Should().BeOfType<IdentifierExpressionSyntax>();
		}

		[Test]
		public void Parser_ChainedAssignments_BindRightToLeft()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("a = b = c = 1");

			var assignA = expr.Should().BeOfType<AssignmentExpressionSyntax>().Subject;
			assignA.Left.Should().BeOfType<IdentifierExpressionSyntax>().Which.IdentifierToken.GetText().Should().Be("a");

			var assignB = assignA.Right.Should().BeOfType<AssignmentExpressionSyntax>().Subject;
			assignB.Left.Should().BeOfType<IdentifierExpressionSyntax>().Which.IdentifierToken.GetText().Should().Be("b");
		}

		[Test]
		public void Parser_TernaryOperator_ShouldParseCorrectly()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("a > b ? x : y");

			var ternary = expr.Should().BeOfType<TernaryExpressionSyntax>().Subject;
			ternary.Condition.Should().BeOfType<BinaryExpressionSyntax>();
			ternary.TrueExpression.Should().BeOfType<IdentifierExpressionSyntax>();
			ternary.FalseExpression.Should().BeOfType<IdentifierExpressionSyntax>();
		}

		[Test]
		public void Parser_NullCoalescing_ShouldParseCorrectly()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("target ?? defaultTarget");

			var coalesce = expr.Should().BeOfType<NullCoalescingExpressionSyntax>().Subject;
			coalesce.Left.Should().BeOfType<IdentifierExpressionSyntax>();
			coalesce.Right.Should().BeOfType<IdentifierExpressionSyntax>();
		}

		[Test]
		public void Parser_RangeExpression_ShouldParseCorrectly()
		{
			var expr = ParserTestExtensions.ParseSingleExpression("0 .. 5 + 1");

			var range = expr.Should().BeOfType<RangeExpressionSyntax>().Subject;
			range.StartExpression.Should().BeOfType<LiteralExpressionSyntax>();
			range.EndExpression.Should().BeOfType<BinaryExpressionSyntax>();
		}

		[Test]
		public void Parser_PrefixUnary_ShouldParseCorrectly()
		{
			var pre = ParserTestExtensions.ParseSingleExpression("++a");
			pre.Should().BeOfType<UnaryExpressionSyntax>();
		}

		[Test]
		public void Parser_LValue_RejectsLiteralAssignments()
		{
			ParserTestExtensions.ParseText("5 = x;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().ContainSingle(d => d.Descriptor.Code == DiagnosticCode.Syn202_InvalidAssignmentTarget);
		}

		[Test]
		public void Parser_LValue_RejectsBinaryExpressionAssignments()
		{
			ParserTestExtensions.ParseText("(a + b) = 10;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().ContainSingle(d => d.Descriptor.Code == DiagnosticCode.Syn202_InvalidAssignmentTarget);
		}

		[Test]
		public void Parser_LValue_RejectsMethodCallAssignments()
		{
			ParserTestExtensions.ParseText("player.GetHealth() = 50;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().ContainSingle(d => d.Descriptor.Code == DiagnosticCode.Syn202_InvalidAssignmentTarget);
		}

		[Test]
		public void Parser_LValue_RejectsInvalidPostfixTargets()
		{
			ParserTestExtensions.ParseText("10++;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeTrue();
			parser.Diagnostics.Diagnostics.Should().ContainSingle(d => d.Descriptor.Code == DiagnosticCode.Syn205_InvalidPostfix);
		}

		[Test]
		public void Parser_LValue_AcceptsValidComplexTargets()
		{
			ParserTestExtensions.ParseText("player.Stats.BaseHealth += 10;", out var parser);

			parser.Diagnostics.HasErrors.Should().BeFalse();
		}

		[Test]
		public void Parser_Operators_ParsesChainedNullConditionalAccess()
		{
			var ast = ParserTestExtensions.ParseText("player?.Inventory?.GetItem();", out var parser);

			parser.Diagnostics.HasErrors.Should().BeFalse();
			var stmt = ast.Statements[0].Should().BeOfType<ExpressionStatementSyntax>().Subject;

			var call = stmt.Expression.Should().BeOfType<CallExpressionSyntax>().Subject;
			var access = call.Identifier.Should().BeOfType<NullConditionalAccessExpressionSyntax>().Subject;

			access.IdentifierToken.GetText().Should().Be("GetItem");
		}
	}
}