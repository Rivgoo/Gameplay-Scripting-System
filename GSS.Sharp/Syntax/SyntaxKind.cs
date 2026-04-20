namespace GSS.Sharp.Syntax
{
	public enum SyntaxKind
	{
		BadToken, EndOfFileToken, WhitespaceToken, CommentToken,
		IdentifierToken, NumberToken, StringToken,
		InterpolatedStringStartToken, InterpolatedStringEndToken, InterpolatedStringTextToken,

		PlusToken, MinusToken, StarToken, SlashToken, PercentToken, BangToken,
		EqualsToken, PlusEqualsToken, MinusEqualsToken, StarEqualsToken, SlashEqualsToken,
		PlusPlusToken, MinusMinusToken, AmpersandAmpersandToken, PipePipeToken,
		QuestionQuestionToken, EqualsEqualsToken, BangEqualsToken, LessToken,
		LessOrEqualsToken, GreaterToken, GreaterOrEqualsToken, OpenParenToken,
		CloseParenToken, OpenBraceToken, CloseBraceToken, OpenBracketToken,
		CloseBracketToken, CommaToken, DotToken,
		DotDotToken, SemicolonToken, QuestionToken, ColonToken, ArrowToken,
		QuestionDotToken,

		ImportKeyword, PublicKeyword, PrivateKeyword, VoidKeyword, IfKeyword,
		ElseKeyword, WhileKeyword, ForKeyword, InKeyword, BreakKeyword,
		ContinueKeyword, ReturnKeyword, TrueKeyword, FalseKeyword, NullKeyword,
		VarKeyword, WaitKeyword,

		CompilationUnit, ImportDirective, MethodDeclaration,
		BlockStatement, ExpressionStatement, VariableDeclaration,
		ImplicitVariableDeclaration, IfStatement, WhileStatement,
		ForStatement, ForEachStatement, BreakStatement, ContinueStatement,
		ReturnStatement, WaitStatement,

		IdentifierName, GenericName,

		LiteralExpression, IdentifierExpression, UnaryExpression,
		PostfixUnaryExpression, BinaryExpression, TernaryExpression,
		NullCoalescingExpression, RangeExpression, CallExpression,
		MemberAccessExpression, NullConditionalAccessExpression,
		ElementAccessExpression, AssignmentExpression, ArrowExpressionClause,
		InterpolatedStringExpression
	}
}