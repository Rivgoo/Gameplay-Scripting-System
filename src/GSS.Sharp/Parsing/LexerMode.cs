namespace GSS.Sharp.Parsing
{
    internal enum LexerMode : byte
    {
        Default,
        InterpolatedString,
        InterpolationExpression
    }
}