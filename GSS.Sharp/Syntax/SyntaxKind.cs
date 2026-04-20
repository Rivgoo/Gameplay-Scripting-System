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
		CloseParenToken, OpenBraceToken, CloseBraceToken, CommaToken, DotToken,
		DotDotToken, SemicolonToken, QuestionToken, ColonToken, ArrowToken,
		QuestionDotToken,

		ImportKeyword, PublicKeyword, PrivateKeyword, VoidKeyword, IfKeyword,
		ElseKeyword, WhileKeyword, ForKeyword, InKeyword, BreakKeyword,
		ContinueKeyword, ReturnKeyword, TrueKeyword, FalseKeyword, NullKeyword, VarKeyword,

		CompilationUnit, ImportDirective, MethodDeclaration,
		BlockStatement, ExpressionStatement, VariableDeclaration,
		ImplicitVariableDeclaration, IfStatement, WhileStatement,
		ForStatement, ForEachStatement, BreakStatement, ContinueStatement, ReturnStatement,

		IdentifierName, GenericName,

		LiteralExpression, IdentifierExpression, UnaryExpression,
		PostfixUnaryExpression, BinaryExpression, TernaryExpression,
		NullCoalescingExpression, RangeExpression, CallExpression,
		MemberAccessExpression, NullConditionalAccessExpression,
		AssignmentExpression, ArrowExpressionClause, InterpolatedStringExpression
	}
}