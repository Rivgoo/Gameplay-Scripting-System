namespace GSS.Core.Compiler.Diagnostics
{
	public enum DiagnosticCode : ushort
	{
		// Lexer (100-199)
		Lex100_UnexpectedCharacter = 100,
		Lex101_UnterminatedString = 101,
		Lex102_UnterminatedComment = 102,
		Lex103_InvalidNumberFormat = 103,
		Lex104_UnexpectedEndOfFile = 104,
		Lex105_InvalidEscapeSequence = 105,
		Lex106_NumericOverflow = 106,

		// Parser (200-299)
		Syn200_UnexpectedToken = 200,
		Syn201_ExpectedExpression = 201,
		Syn202_InvalidAssignmentTarget = 202,
		Syn203_TooManyNestedExpressions = 203,
		Syn204_MissingToken = 204,
		Syn205_InvalidPostfix = 205,
		Syn206_SkippedInvalidTokens = 206,

		// Semantic / Binder (300-399)
		Sem300_TypeMismatch = 300,
		Sem301_UndefinedVariable = 301,
		Sem302_VariableAlreadyDeclared = 302,
		Sem303_CannotAssignReadOnly = 303,
		Sem304_UndefinedMethod = 304,
		Sem305_ArgumentCountMismatch = 305,
		Sem306_InvalidOperator = 306,
		Sem307_ImplicitVarWithoutInitializer = 307,
		Sem308_InvalidAssignmentTarget = 308,
		Sem309_UninitializedVariable = 309,
		Sem310_InvalidBreakContext = 310,
		Sem311_InvalidContinueContext = 311,
		Sem312_FeatureNotSupported = 312
	}
}