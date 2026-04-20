namespace GSS.Sharp.Binding.Nodes
{
	public enum BoundNodeKind
	{
		LiteralExpression, VariableExpression, ApiPropertyExpression,
		AssignmentExpression, ApiAssignmentExpression, UnaryExpression,
		BinaryExpression, CallExpression, ConversionExpression,
		IndexExpression, IndexAssignmentExpression,
		TernaryExpression,

		BlockStatement, ExpressionStatement, VariableDeclaration,
		IfStatement, WhileStatement, ReturnStatement, LabelStatement,
		GotoStatement, ConditionalGotoStatement, WaitStatement
	}
}