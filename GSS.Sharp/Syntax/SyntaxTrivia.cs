using GSS.Core.Compiler.Diagnostics;

namespace GSS.Sharp.Syntax
{
	public readonly struct SyntaxTrivia
	{
		public SyntaxKind Kind { get; }
		public TextSpan Span { get; }
		public string Text { get; }

		public SyntaxTrivia(SyntaxKind kind, TextSpan span, string text)
		{
			Kind = kind;
			Span = span;
			Text = text;
		}
	}
}