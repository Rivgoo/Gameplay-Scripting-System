namespace GSS.Core.Compiler.Diagnostics
{
	public enum DiagnosticCode
	{
		// Lexer (100-199)
		Lex100_UnexpectedCharacter,
		Lex101_UnterminatedString,
		Lex102_UnterminatedComment,
		Lex103_InvalidNumberFormat,
		Lex104_UnexpectedEndOfFile,
		Lex105_InvalidEscapeSequence,
		Lex106_NumericOverflow,

		// Parser (200-299)
		Syn200_UnexpectedToken,
		Syn201_ExpectedExpression,
		Syn202_InvalidAssignmentTarget,
		Syn203_TooManyNestedExpressions,
		Syn204_MissingToken,
		Syn205_InvalidPostfix,
		Syn206_SkippedInvalidTokens
	}
}