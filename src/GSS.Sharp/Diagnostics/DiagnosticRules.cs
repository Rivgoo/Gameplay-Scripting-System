using GSS.Kernel.Diagnostics;

namespace GSS.Sharp.Diagnostics
{
    public static class DiagnosticRules
    {
        public static readonly DiagnosticDescriptor LexUnexpectedChar = new(
            DiagnosticCode.Lex100_UnexpectedCharacter, DiagnosticSeverity.Error,
            "Unexpected character '{0}'.", "Remove or replace the invalid character.");

        public static readonly DiagnosticDescriptor LexUnterminatedString = new(
            DiagnosticCode.Lex101_UnterminatedString, DiagnosticSeverity.Error,
            "Unterminated string literal.", "Add a closing double quote (\").");

        public static readonly DiagnosticDescriptor LexUnterminatedComment = new(
            DiagnosticCode.Lex102_UnterminatedComment, DiagnosticSeverity.Error,
            "Unterminated block comment.", "Add '*/' to close the comment block.");

        public static readonly DiagnosticDescriptor LexInvalidNumber = new(
            DiagnosticCode.Lex103_InvalidNumberFormat, DiagnosticSeverity.Error,
            "Invalid number format '{0}'.", "Ensure the number uses correct syntax.");

        public static readonly DiagnosticDescriptor LexUnexpectedEof = new(
            DiagnosticCode.Lex104_UnexpectedEndOfFile, DiagnosticSeverity.Error,
            "Unexpected end of file.", "Complete the final expression or close open blocks.");

        public static readonly DiagnosticDescriptor LexInvalidEscape = new(
            DiagnosticCode.Lex105_InvalidEscapeSequence, DiagnosticSeverity.Error,
            "Invalid escape sequence '\\{0}'.", "Use standard escape sequences (e.g., \\n, \\t, \\\").");

        public static readonly DiagnosticDescriptor LexNumericOverflow = new(
            DiagnosticCode.Lex106_NumericOverflow, DiagnosticSeverity.Error,
            "Number '{0}' exceeds maximum capacity.", "Use a smaller number or a different numeric type.");

        public static readonly DiagnosticDescriptor SynUnexpectedToken = new(
            DiagnosticCode.Syn200_UnexpectedToken, DiagnosticSeverity.Error,
            "Unexpected token <{0}>, expected <{1}>.", "Verify syntax context.");

        public static readonly DiagnosticDescriptor SynExpectedExpression = new(
            DiagnosticCode.Syn201_ExpectedExpression, DiagnosticSeverity.Error,
            "Expected an expression, but found <{0}>.", "Provide a valid expression.");

        public static readonly DiagnosticDescriptor SynInvalidAssignment = new(
            DiagnosticCode.Syn202_InvalidAssignmentTarget, DiagnosticSeverity.Error,
            "Invalid assignment target '{0}'.", "Assign values only to variables or properties.");

        public static readonly DiagnosticDescriptor SynRecursionLimit = new(
            DiagnosticCode.Syn203_TooManyNestedExpressions, DiagnosticSeverity.Error,
            "Maximum expression depth exceeded.", "Simplify the expression.");

        public static readonly DiagnosticDescriptor SynMissingToken = new(
            DiagnosticCode.Syn204_MissingToken, DiagnosticSeverity.Error,
            "Expected <{0}>.", "Insert the missing token.");

        public static readonly DiagnosticDescriptor SynInvalidPostfix = new(
            DiagnosticCode.Syn205_InvalidPostfix, DiagnosticSeverity.Error,
            "Invalid target '{0}' for postfix operator.", "Apply postfix operators only to variables or properties.");

        public static readonly DiagnosticDescriptor SynSkippedTokens = new(
            DiagnosticCode.Syn206_SkippedInvalidTokens, DiagnosticSeverity.Warning,
            "Skipped invalid tokens during error recovery.", "Fix preceding syntax errors.");
    }
}