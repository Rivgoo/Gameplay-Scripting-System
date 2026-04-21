using GSS.Kernel.Compiler.Syntax;
using GSS.Kernel.Diagnostics;
using GSS.Kernel.Text;

namespace GSS.Sharp.Syntax
{
    public readonly struct SyntaxToken : IToken
    {
        public SyntaxKind Kind { get; }
        public object? Value { get; }
        public ISourceText Source { get; }
        public IReadOnlyList<SyntaxTrivia> LeadingTrivia { get; }
        public IReadOnlyList<SyntaxTrivia> TrailingTrivia { get; }

        private readonly StringSegment _segment;

        public int Type => (int)Kind;
        public int Position => _segment.Start;
        public int Length => _segment.Length;
        public string Text => GetTextSpan().ToString();
        public TextSpan Span => new TextSpan(_segment.Start, _segment.Length);
        public TextLocation Location => new TextLocation(Source, Span);
        public bool IsMissing => _segment.Length == 0;

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
            _segment = new StringSegment(position, length);
            Source = source;
            Value = value;
            LeadingTrivia = leadingTrivia ?? Array.Empty<SyntaxTrivia>();
            TrailingTrivia = trailingTrivia ?? Array.Empty<SyntaxTrivia>();
        }

        public ReadOnlySpan<char> GetTextSpan()
        {
            if (_segment.Length == 0 || Source == null) return ReadOnlySpan<char>.Empty;
            return ((SourceText)Source).GetSpan(_segment);
        }
    }
}