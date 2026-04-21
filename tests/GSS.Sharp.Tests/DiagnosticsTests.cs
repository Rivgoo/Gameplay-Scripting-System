using FluentAssertions;
using GSS.Sharp.Tests.Helpers;

namespace GSS.Sharp.Tests
{
	[TestFixture]
	public class DiagnosticsTests
	{
		[Test]
		public void Diagnostics_LocationMapping_ShouldReportOffendingTokenExactLocation()
		{
			string code = "var a = 1;\nvar b = 2 \nvar c = 3;";
			ParserTestExtensions.ParseText(code, out var parser);

			var error = parser.Diagnostics.Diagnostics[0];

			error.Location.Line.Should().Be(3);
			error.Location.Column.Should().Be(1);
		}

		[Test]
		public void Diagnostics_MessageFormatting_ShouldInjectArguments()
		{
			string code = "if (true) {";
			ParserTestExtensions.ParseText(code, out var parser);

			var error = parser.Diagnostics.Diagnostics[0];

			error.FormattedMessage.Should().Contain("CloseBraceToken");
		}

		[Test]
		public void Diagnostics_SeverityLevel_ShouldMapCorrectly()
		{
			string code = "int a = 1; @; { }";
			ParserTestExtensions.ParseText(code, out var parser);

			var errors = parser.Diagnostics.Diagnostics;

			errors.Should().Contain(d => d.Descriptor.Severity == DiagnosticSeverity.Error);
			errors.Should().Contain(d => d.Descriptor.Severity == DiagnosticSeverity.Warning && d.Descriptor.Code == DiagnosticCode.Syn206_SkippedInvalidTokens);
		}
	}
}