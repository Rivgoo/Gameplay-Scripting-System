using System.Globalization;
using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Text;
using GSS.Sharp.Syntax;

namespace GSS.Sharp.Parsing
{
	public sealed class Lexer
	{
		public DiagnosticBag Diagnostics { get; } = new();
		private readonly ISourceText _source;
		private int _position;
		private readonly Stack<LexerMode> _modeStack = new();
		private int _interpolationBraceDepth;

		public Lexer(ISourceText source)
		{
			_source = source;
			_modeStack.Push(LexerMode.Default);
		}

		public SyntaxToken NextToken()
		{
			var leadingTrivia = ParseTrivia();

			if (_position >= _source.Length)
				return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, 0, _source, null, leadingTrivia);

			SyntaxToken token;
			LexerMode currentMode = _modeStack.Peek();

			if (currentMode == LexerMode.InterpolatedString)
				token = ReadInterpolatedStringText();
			else
				token = ParseStandardToken();

			var trailingTrivia = ParseTrivia(isTrailing: true);
			return new SyntaxToken(token.Kind, token.Position, token.Length, _source, token.Value, leadingTrivia, trailingTrivia);
		}

		private char Current() => Peek(0);
		private char Lookahead() => Peek(1);

		private char Peek(int offset)
		{
			int index = _position + offset;
			if (index >= _source.Length) return '\0';
			return _source.RawText[index];
		}

		private TextLocation CreateLocation(int start, int length) => new(_source, new TextSpan(start, length));

		private List<SyntaxTrivia> ParseTrivia(bool isTrailing = false)
		{
			var trivia = new List<SyntaxTrivia>();
			while (true)
			{
				int start = _position;
				char current = Current();

				if (char.IsWhiteSpace(current))
				{
					while (char.IsWhiteSpace(Current())) _position++;
					trivia.Add(new SyntaxTrivia(SyntaxKind.WhitespaceToken, new TextSpan(start, _position - start), _source.RawText.Substring(start, _position - start)));
					if (isTrailing && (current == '\n' || current == '\r')) break;
				}
				else if (current == '/' && (Lookahead() == '/' || Lookahead() == '*'))
				{
					if (Lookahead() == '/')
					{
						_position += 2;
						while (Current() != '\r' && Current() != '\n' && Current() != '\0') _position++;
					}
					else
					{
						_position += 2;
						while (Current() != '\0' && !(Current() == '*' && Lookahead() == '/')) _position++;
						if (Current() == '\0') Diagnostics.Report(DiagnosticRules.LexUnterminatedComment, CreateLocation(start, _position - start));
						else _position += 2;
					}
					trivia.Add(new SyntaxTrivia(SyntaxKind.CommentToken, new TextSpan(start, _position - start), _source.RawText.Substring(start, _position - start)));
				}
				else break;
			}
			return trivia;
		}

		private SyntaxToken ParseStandardToken()
		{
			char current = Current();

			if (char.IsLetter(current) || current == '_') return ReadIdentifierOrKeyword();
			if (char.IsDigit(current)) return ReadNumber();
			if (current == '"') return ReadString();
			if (current == '$' && Lookahead() == '"') return ReadInterpolatedStringStart();

			return ReadPunctuation();
		}

		private SyntaxToken ReadIdentifierOrKeyword()
		{
			int start = _position;
			while (char.IsLetterOrDigit(Current()) || Current() == '_') _position++;
			int length = _position - start;
			ReadOnlySpan<char> textSpan = _source.RawText.AsSpan(start, length);
			return new SyntaxToken(SyntaxFacts.GetKeywordKind(textSpan), start, length, _source);
		}

		private SyntaxToken ReadNumber()
		{
			int start = _position;
			bool hasDot = false;

			while (char.IsDigit(Current()) || (Current() == '.' && char.IsDigit(Lookahead())))
			{
				if (Current() == '.') hasDot = true;
				_position++;
			}

			int length = _position - start;
			ReadOnlySpan<char> textSpan = _source.RawText.AsSpan(start, length);
			object value = 0;

			if (hasDot)
			{
				if (float.TryParse(textSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out float fVal))
					value = fVal;
				else
					Diagnostics.Report(DiagnosticRules.LexNumericOverflow, CreateLocation(start, length), textSpan.ToString());
			}
			else
			{
				if (int.TryParse(textSpan, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iVal))
					value = iVal;
				else
					Diagnostics.Report(DiagnosticRules.LexNumericOverflow, CreateLocation(start, length), textSpan.ToString());
			}

			return new SyntaxToken(SyntaxKind.NumberToken, start, length, _source, value);
		}

		private SyntaxToken ReadString()
		{
			int start = _position++;
			bool done = false;

			while (!done)
			{
				switch (Current())
				{
					case '\0':
					case '\r':
					case '\n':
						Diagnostics.Report(DiagnosticRules.LexUnterminatedString, CreateLocation(start, _position - start));
						done = true;
						break;
					case '"':
						_position++;
						done = true;
						break;
					case '\\':
						_position++;
						char esc = Current();
						if (esc != 'n' && esc != 'r' && esc != 't' && esc != '\\' && esc != '"' && esc != '\0')
							Diagnostics.Report(DiagnosticRules.LexInvalidEscape, CreateLocation(_position - 1, 2), esc);
						if (esc != '\0') _position++;
						break;
					default:
						_position++;
						break;
				}
			}
			return new SyntaxToken(SyntaxKind.StringToken, start, _position - start, _source);
		}

		private SyntaxToken ReadInterpolatedStringStart()
		{
			int start = _position;
			_position += 2;
			_modeStack.Push(LexerMode.InterpolatedString);
			return new SyntaxToken(SyntaxKind.InterpolatedStringStartToken, start, 2, _source);
		}

		private SyntaxToken ReadInterpolatedStringText()
		{
			int start = _position;

			if (Current() == '"')
			{
				_position++;
				_modeStack.Pop();
				return new SyntaxToken(SyntaxKind.InterpolatedStringEndToken, start, 1, _source);
			}

			if (Current() == '{')
			{
				_position++;
				_modeStack.Push(LexerMode.InterpolationExpression);
				_interpolationBraceDepth++;
				return new SyntaxToken(SyntaxKind.OpenBraceToken, start, 1, _source);
			}

			while (Current() != '"' && Current() != '{' && Current() != '\0')
			{
				if (Current() == '\\') _position++;
				_position++;
			}

			if (Current() == '\0')
			{
				Diagnostics.Report(DiagnosticRules.LexUnterminatedString, CreateLocation(start, _position - start));
				_modeStack.Pop();
			}

			return new SyntaxToken(SyntaxKind.InterpolatedStringTextToken, start, _position - start, _source);
		}

		private SyntaxToken ReadPunctuation()
		{
			int start = _position;
			char current = Current();
			char lookahead = Lookahead();

			_position++;

			switch (current)
			{
				case '+':
					if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.PlusEqualsToken, start, 2, _source); }
					if (lookahead == '+') { _position++; return new SyntaxToken(SyntaxKind.PlusPlusToken, start, 2, _source); }
					return new SyntaxToken(SyntaxKind.PlusToken, start, 1, _source);
				case '-':
					if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.MinusEqualsToken, start, 2, _source); }
					if (lookahead == '-') { _position++; return new SyntaxToken(SyntaxKind.MinusMinusToken, start, 2, _source); }
					return new SyntaxToken(SyntaxKind.MinusToken, start, 1, _source);
				case '*': if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.StarEqualsToken, start, 2, _source); } return new SyntaxToken(SyntaxKind.StarToken, start, 1, _source);
				case '/': if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.SlashEqualsToken, start, 2, _source); } return new SyntaxToken(SyntaxKind.SlashToken, start, 1, _source);
				case '%': return new SyntaxToken(SyntaxKind.PercentToken, start, 1, _source);
				case '(': return new SyntaxToken(SyntaxKind.OpenParenToken, start, 1, _source);
				case ')': return new SyntaxToken(SyntaxKind.CloseParenToken, start, 1, _source);
				case '{':
					if (_modeStack.Peek() == LexerMode.InterpolationExpression) _interpolationBraceDepth++;
					return new SyntaxToken(SyntaxKind.OpenBraceToken, start, 1, _source);
				case '}':
					if (_modeStack.Peek() == LexerMode.InterpolationExpression)
					{
						_interpolationBraceDepth--;
						if (_interpolationBraceDepth == 0) _modeStack.Pop();
					}
					return new SyntaxToken(SyntaxKind.CloseBraceToken, start, 1, _source);
				case ',': return new SyntaxToken(SyntaxKind.CommaToken, start, 1, _source);
				case ';': return new SyntaxToken(SyntaxKind.SemicolonToken, start, 1, _source);
				case '.': if (lookahead == '.') { _position++; return new SyntaxToken(SyntaxKind.DotDotToken, start, 2, _source); } return new SyntaxToken(SyntaxKind.DotToken, start, 1, _source);
				case '=':
					if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, 2, _source); }
					if (lookahead == '>') { _position++; return new SyntaxToken(SyntaxKind.ArrowToken, start, 2, _source); }
					return new SyntaxToken(SyntaxKind.EqualsToken, start, 1, _source);
				case '!': if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.BangEqualsToken, start, 2, _source); } return new SyntaxToken(SyntaxKind.BangToken, start, 1, _source);
				case '<': if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.LessOrEqualsToken, start, 2, _source); } return new SyntaxToken(SyntaxKind.LessToken, start, 1, _source);
				case '>': if (lookahead == '=') { _position++; return new SyntaxToken(SyntaxKind.GreaterOrEqualsToken, start, 2, _source); } return new SyntaxToken(SyntaxKind.GreaterToken, start, 1, _source);
				case '&': if (lookahead == '&') { _position++; return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, 2, _source); } break;
				case '|': if (lookahead == '|') { _position++; return new SyntaxToken(SyntaxKind.PipePipeToken, start, 2, _source); } break;
				case '?':
					if (lookahead == '?') { _position++; return new SyntaxToken(SyntaxKind.QuestionQuestionToken, start, 2, _source); }
					if (lookahead == '.') { _position++; return new SyntaxToken(SyntaxKind.QuestionDotToken, start, 2, _source); }
					return new SyntaxToken(SyntaxKind.QuestionToken, start, 1, _source);
				case ':': return new SyntaxToken(SyntaxKind.ColonToken, start, 1, _source);
			}

			Diagnostics.Report(DiagnosticRules.LexUnexpectedChar, CreateLocation(start, 1), current);
			return new SyntaxToken(SyntaxKind.BadToken, start, 1, _source);
		}
	}
}