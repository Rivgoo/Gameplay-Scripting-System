namespace GSS.Sharp.Syntax
{
    public static class SyntaxFacts
    {
        public static SyntaxKind GetKeywordKind(ReadOnlySpan<char> text)
        {
            return text switch
            {
                "import" => SyntaxKind.ImportKeyword,
                "public" => SyntaxKind.PublicKeyword,
                "private" => SyntaxKind.PrivateKeyword,
                "void" => SyntaxKind.VoidKeyword,
                "if" => SyntaxKind.IfKeyword,
                "else" => SyntaxKind.ElseKeyword,
                "while" => SyntaxKind.WhileKeyword,
                "for" => SyntaxKind.ForKeyword,
                "in" => SyntaxKind.InKeyword,
                "break" => SyntaxKind.BreakKeyword,
                "continue" => SyntaxKind.ContinueKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "null" => SyntaxKind.NullKeyword,
                "var" => SyntaxKind.VarKeyword,
                _ => SyntaxKind.IdentifierToken
            };
        }

        public static int GetUnaryOperatorPrecedence(SyntaxKind kind) => kind switch
        {
            SyntaxKind.PlusToken or
            SyntaxKind.MinusToken or
            SyntaxKind.BangToken or
            SyntaxKind.PlusPlusToken or
            SyntaxKind.MinusMinusToken => 9,
            _ => 0
        };

        public static int GetBinaryOperatorPrecedence(SyntaxKind kind) => kind switch
        {
            SyntaxKind.StarToken or SyntaxKind.SlashToken or SyntaxKind.PercentToken => 8,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 7,
            SyntaxKind.LessToken or SyntaxKind.LessOrEqualsToken or SyntaxKind.GreaterToken or SyntaxKind.GreaterOrEqualsToken => 6,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken => 5,
            SyntaxKind.AmpersandAmpersandToken => 4,
            SyntaxKind.PipePipeToken => 3,
            SyntaxKind.QuestionQuestionToken => 2,
            SyntaxKind.QuestionToken => 1,
            _ => 0
        };

        public static bool IsAssignmentOperator(SyntaxKind kind) => kind switch
        {
            SyntaxKind.EqualsToken or
            SyntaxKind.PlusEqualsToken or
            SyntaxKind.MinusEqualsToken or
            SyntaxKind.StarEqualsToken or
            SyntaxKind.SlashEqualsToken => true,
            _ => false
        };
    }
}