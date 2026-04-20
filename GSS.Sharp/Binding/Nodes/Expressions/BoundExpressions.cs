using GSS.Core.ApiBinding.Abstractions;
using GSS.Sharp.Binding.Symbols;

namespace GSS.Sharp.Binding.Nodes.Expressions
{
	public sealed class BoundLiteralExpression : BoundExpression
	{
		public object? Value { get; }
		public override TypeSymbol Type { get; }
		public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

		public BoundLiteralExpression(object? value, TypeSymbol type)
		{
			Value = value;
			Type = type;
		}
	}

	public sealed class BoundVariableExpression : BoundExpression
	{
		public VariableSymbol Variable { get; }
		public override TypeSymbol Type => Variable.Type;
		public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;

		public BoundVariableExpression(VariableSymbol variable) => Variable = variable;
	}

	public sealed class BoundApiPropertyExpression : BoundExpression
	{
		public IPropertyMetadata Property { get; }
		public override TypeSymbol Type { get; }
		public override BoundNodeKind Kind => BoundNodeKind.ApiPropertyExpression;

		public BoundApiPropertyExpression(IPropertyMetadata property, TypeSymbol type)
		{
			Property = property;
			Type = type;
		}
	}

	public sealed class BoundAssignmentExpression : BoundExpression
	{
		public VariableSymbol Variable { get; }
		public BoundExpression Expression { get; }
		public override TypeSymbol Type => Expression.Type;
		public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

		public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
		{
			Variable = variable;
			Expression = expression;
		}
	}

	public sealed class BoundApiAssignmentExpression : BoundExpression
	{
		public IPropertyMetadata Property { get; }
		public BoundExpression Expression { get; }
		public override TypeSymbol Type => Expression.Type;
		public override BoundNodeKind Kind => BoundNodeKind.ApiAssignmentExpression;

		public BoundApiAssignmentExpression(IPropertyMetadata property, BoundExpression expression)
		{
			Property = property;
			Expression = expression;
		}
	}

	public sealed class BoundCallExpression : BoundExpression
	{
		public IMethodMetadata Method { get; }
		public IReadOnlyList<BoundExpression> Arguments { get; }
		public override TypeSymbol Type { get; }
		public override BoundNodeKind Kind => BoundNodeKind.CallExpression;

		public BoundCallExpression(IMethodMetadata method, IReadOnlyList<BoundExpression> arguments, TypeSymbol type)
		{
			Method = method;
			Arguments = arguments;
			Type = type;
		}
	}

	public sealed class BoundConversionExpression : BoundExpression
	{
		public BoundExpression Expression { get; }
		public override TypeSymbol Type { get; }
		public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;

		public BoundConversionExpression(TypeSymbol type, BoundExpression expression)
		{
			Type = type;
			Expression = expression;
		}
	}

	public sealed class BoundIndexExpression : BoundExpression
	{
		public BoundExpression Collection { get; }
		public BoundExpression Index { get; }
		public override TypeSymbol Type { get; }
		public override BoundNodeKind Kind => BoundNodeKind.IndexExpression;

		public BoundIndexExpression(BoundExpression collection, BoundExpression index, TypeSymbol type)
		{
			Collection = collection;
			Index = index;
			Type = type;
		}
	}

	public sealed class BoundIndexAssignmentExpression : BoundExpression
	{
		public BoundExpression Collection { get; }
		public BoundExpression Index { get; }
		public BoundExpression Expression { get; }
		public override TypeSymbol Type => Expression.Type;
		public override BoundNodeKind Kind => BoundNodeKind.IndexAssignmentExpression;

		public BoundIndexAssignmentExpression(BoundExpression collection, BoundExpression index, BoundExpression expression)
		{
			Collection = collection;
			Index = index;
			Expression = expression;
		}
	}

	public sealed class BoundTernaryExpression : BoundExpression
	{
		public BoundExpression Condition { get; }
		public BoundExpression TrueExpression { get; }
		public BoundExpression FalseExpression { get; }
		public override TypeSymbol Type => TrueExpression.Type;
		public override BoundNodeKind Kind => BoundNodeKind.TernaryExpression;

		public BoundTernaryExpression(BoundExpression condition, BoundExpression trueExpression, BoundExpression falseExpression)
		{
			Condition = condition;
			TrueExpression = trueExpression;
			FalseExpression = falseExpression;
		}
	}
}