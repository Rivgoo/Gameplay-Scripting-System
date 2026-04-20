using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Syntax;
using GSS.Core.Compiler.Text;

namespace GSS.Sharp.Syntax
{
	public readonly struct SyntaxToken : IToken
	{
		public SyntaxKind Kind { get; }
		public int Position { get; }
		public int Length { get; }
		public object? Value { get; }
		public ISourceText Source { get; }
		public IReadOnlyList<SyntaxTrivia> LeadingTrivia { get; }
		public IReadOnlyList<SyntaxTrivia> TrailingTrivia { get; }

		public int Type => (int)Kind;
		public string Text => GetText();
		public TextSpan Span => new TextSpan(Position, Length);
		public TextLocation Location => new TextLocation(Source, Span);
		public bool IsMissing => Length == 0;

		public SyntaxToken(
			SyntaxKind kind,
			int position,
			int length,
			ISourceText source,
			object? value = null,
			IReadOnlyList<SyntaxTrivia>? leadingTrivia = null,
			IReadOnlyList<SyntaxTrivia>? trailingTrivia = null)
		{
			Kind = kind;
			Position = position;
			Length = length;
			Source = source;
			Value = value;
			LeadingTrivia = leadingTrivia ?? Array.Empty<SyntaxTrivia>();
			TrailingTrivia = trailingTrivia ?? Array.Empty<SyntaxTrivia>();
		}

		public string GetText()
		{
			if (Length == 0 || Source == null) return string.Empty;
			return Source.RawText.Substring(Position, Length);
		}
	}
}