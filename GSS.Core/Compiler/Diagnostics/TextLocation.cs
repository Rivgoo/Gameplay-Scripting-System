using GSS.Core.Compiler.Text;

namespace GSS.Core.Compiler.Diagnostics
{
	public readonly struct TextLocation
	{
		public ISourceText Source { get; }
		public TextSpan Span { get; }

		public int Line => Source.GetLineIndex(Span.Start) + 1;
		public int Column => Source.GetCharacterOffset(Span.Start) + 1;

		public TextLocation(ISourceText source, TextSpan span)
		{
			Source = source;
			Span = span;
		}

		public override string ToString() => $"Line {Line}, Column {Column}";
	}
}