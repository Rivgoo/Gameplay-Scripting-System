using GSS.Sharp.Syntax;

namespace GSS.Sharp.Parsing
{
    internal sealed class TokenBuffer
    {
        private readonly Lexer _lexer;
        private readonly List<SyntaxToken> _buffer = new();

        public TokenBuffer(Lexer lexer) => _lexer = lexer;

        public SyntaxToken Peek(int offset)
        {
            while (_buffer.Count <= offset)
            {
                if (_buffer.Count > 0 && _buffer[^1].Kind == SyntaxKind.EndOfFileToken)
                    return _buffer[^1];

                SyntaxToken token;
                do
                {
                    token = _lexer.NextToken();
                } while (token.Kind == SyntaxKind.BadToken);

                _buffer.Add(token);
            }
            return _buffer[offset];
        }

        public SyntaxToken Consume()
        {
            SyntaxToken token = Peek(0);
            if (token.Kind != SyntaxKind.EndOfFileToken) _buffer.RemoveAt(0);
            return token;
        }
    }
}