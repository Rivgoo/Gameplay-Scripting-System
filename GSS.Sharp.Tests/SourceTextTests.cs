using FluentAssertions;
using GSS.Core.Compiler.Text;

namespace GSS.Sharp.Tests
{
	[TestFixture]
	public class SourceTextTests
	{
		[Test]
		public void SourceText_GivenEmptyString_ShouldHandleGracefully()
		{
			var source = new SourceText("");
			source.Length.Should().Be(0);
			source.GetLineIndex(0).Should().Be(0);
			source.GetCharacterOffset(0).Should().Be(0);
		}

		[Test]
		public void SourceText_GivenUnixAndWindowsLineEndings_ShouldMapCorrectly()
		{
			string text = "Line1\nLine2\r\nLine3";
			var source = new SourceText(text);

			source.GetLineIndex(0).Should().Be(0);
			source.GetLineIndex(5).Should().Be(0);
			source.GetLineIndex(6).Should().Be(1);
			source.GetLineIndex(12).Should().Be(1);
			source.GetLineIndex(13).Should().Be(2);
		}

		[Test]
		public void SourceText_GivenOutOfBoundsIndex_ShouldClampToLastLine()
		{
			string text = "A\nB";
			var source = new SourceText(text);

			source.GetLineIndex(999).Should().Be(1);
		}

		[Test]
		public void SourceText_GivenOnlyNewLines_ShouldMapCorrectly()
		{
			string text = "\n\n\n";
			var source = new SourceText(text);

			source.GetLineIndex(0).Should().Be(0);
			source.GetLineIndex(1).Should().Be(1);
			source.GetLineIndex(2).Should().Be(2);
			source.GetLineIndex(3).Should().Be(3);
		}

		[Test]
		public void SourceText_GivenSingleLongLine_ShouldReturnZeroIndexForAllOffsets()
		{
			string text = new string('A', 1000);
			var source = new SourceText(text);

			source.GetLineIndex(500).Should().Be(0);
			source.GetCharacterOffset(500).Should().Be(500);
		}
	}
}