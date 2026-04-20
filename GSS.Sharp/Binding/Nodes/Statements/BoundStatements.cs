using GSS.Sharp.Binding.Nodes.Expressions;
using GSS.Sharp.Binding.Symbols;

namespace GSS.Sharp.Binding.Nodes.Statements
{
	public sealed class BoundBlockStatement : BoundStatement
	{
		public IReadOnlyList<BoundStatement> Statements { get; }
		public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
		public BoundBlockStatement(IReadOnlyList<BoundStatement> statements) => Statements = statements;
	}

	public sealed class BoundExpressionStatement : BoundStatement
	{
		public BoundExpression Expression { get; }
		public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;
		public BoundExpressionStatement(BoundExpression expression) => Expression = expression;
	}

	public sealed class BoundVariableDeclaration : BoundStatement
	{
		public VariableSymbol Variable { get; }
		public BoundExpression Initializer { get; }
		public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;
		public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initializer)
		{
			Variable = variable;
			Initializer = initializer;
		}
	}

	public sealed class BoundIfStatement : BoundStatement
	{
		public BoundExpression Condition { get; }
		public BoundStatement ThenStatement { get; }
		public BoundStatement? ElseStatement { get; }
		public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
		public BoundIfStatement(BoundExpression condition, BoundStatement thenStatement, BoundStatement? elseStatement)
		{
			Condition = condition;
			ThenStatement = thenStatement;
			ElseStatement = elseStatement;
		}
	}

	public sealed class BoundWhileStatement : BoundStatement
	{
		public BoundExpression Condition { get; }
		public BoundStatement Body { get; }
		public BoundLabel BreakLabel { get; }
		public BoundLabel ContinueLabel { get; }
		public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;
		public BoundWhileStatement(BoundExpression condition, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel)
		{
			Condition = condition;
			Body = body;
			BreakLabel = breakLabel;
			ContinueLabel = continueLabel;
		}
	}

	public sealed class BoundBinaryExpression : BoundExpression
	{
		public BoundExpression Left { get; }
		public BoundBinaryOperator Op { get; }
		public BoundExpression Right { get; }
		public override TypeSymbol Type => Op.ResultType;
		public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;

		public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
		{
			Left = left;
			Op = op;
			Right = right;
		}
	}

	public sealed class BoundUnaryExpression : BoundExpression
	{
		public BoundUnaryOperator Op { get; }
		public BoundExpression Operand { get; }
		public override TypeSymbol Type => Op.ResultType;
		public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

		public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
		{
			Op = op;
			Operand = operand;
		}
	}

	public sealed class BoundReturnStatement : BoundStatement
	{
		public BoundExpression? Expression { get; }
		public override BoundNodeKind Kind => BoundNodeKind.ReturnStatement;
		public BoundReturnStatement(BoundExpression? expression) => Expression = expression;
	}

	public sealed class BoundLabel
	{
		public string Name { get; }
		public BoundLabel(string name) => Name = name;
		public override string ToString() => Name;
	}

	public sealed class BoundLabelStatement : BoundStatement
	{
		public BoundLabel Label { get; }
		public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
		public BoundLabelStatement(BoundLabel label) => Label = label;
	}

	public sealed class BoundGotoStatement : BoundStatement
	{
		public BoundLabel Label { get; }
		public override BoundNodeKind Kind => BoundNodeKind.GotoStatement;
		public BoundGotoStatement(BoundLabel label) => Label = label;
	}

	public sealed class BoundConditionalGotoStatement : BoundStatement
	{
		public BoundLabel Label { get; }
		public BoundExpression Condition { get; }
		public bool JumpIfTrue { get; }
		public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;

		public BoundConditionalGotoStatement(BoundLabel label, BoundExpression condition, bool jumpIfTrue)
		{
			Label = label;
			Condition = condition;
			JumpIfTrue = jumpIfTrue;
		}
	}

	public sealed class BoundWaitStatement : BoundStatement
	{
		public BoundExpression Duration { get; }
		public override BoundNodeKind Kind => BoundNodeKind.WaitStatement;

		public BoundWaitStatement(BoundExpression duration)
		{
			Duration = duration;
		}
	}
}