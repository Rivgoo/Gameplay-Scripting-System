using GSS.Sharp.Syntax;
using GSS.Sharp.Binding.Symbols;

namespace GSS.Sharp.Binding.Nodes.Expressions
{
    public sealed class BoundUnaryOperator
    {
        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            ResultType = resultType;
        }

        private static readonly BoundUnaryOperator[] _operators =
        {
            new(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool, TypeSymbol.Bool),
            new(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int, TypeSymbol.Int),
            new(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int, TypeSymbol.Int),
            new(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Float, TypeSymbol.Float),
            new(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Float, TypeSymbol.Float)
        };

        public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
        {
            for (int i = 0; i < _operators.Length; i++)
            {
                if (_operators[i].SyntaxKind == syntaxKind && _operators[i].OperandType == operandType)
                    return _operators[i];
            }
            return null;
        }
    }

    public sealed class BoundBinaryOperator
    {
        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResultType { get; }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type)
            : this(syntaxKind, kind, type, type, type) { }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
            : this(syntaxKind, kind, operandType, operandType, resultType) { }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        private static readonly BoundBinaryOperator[] _operators =
        {
            // Int Operations
            new(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Int),
            new(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Int),
            new(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int),
            new(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, TypeSymbol.Int),
            new(SyntaxKind.PercentToken, BoundBinaryOperatorKind.Modulo, TypeSymbol.Int),
            new(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxKind.LessToken, BoundBinaryOperatorKind.Less, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.Greater, TypeSymbol.Int, TypeSymbol.Bool),
            new(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEquals, TypeSymbol.Int, TypeSymbol.Bool),

            // Float Operations
            new(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Float),
            new(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Float),
            new(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Float),
            new(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, TypeSymbol.Float),
            new(SyntaxKind.PercentToken, BoundBinaryOperatorKind.Modulo, TypeSymbol.Float),
            new(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Float, TypeSymbol.Bool),
            new(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Float, TypeSymbol.Bool),
            new(SyntaxKind.LessToken, BoundBinaryOperatorKind.Less, TypeSymbol.Float, TypeSymbol.Bool),
            new(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEquals, TypeSymbol.Float, TypeSymbol.Bool),
            new(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.Greater, TypeSymbol.Float, TypeSymbol.Bool),
            new(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEquals, TypeSymbol.Float, TypeSymbol.Bool),

            // Vector Operations
            new(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Vector),
            new(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Vector),
            new(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Vector, TypeSymbol.Float, TypeSymbol.Vector), // Vector * Float = Vector

            // String Operations
            new(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.String),
            new(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.String, TypeSymbol.Bool),
            new(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.String, TypeSymbol.Bool),

            // Bool Operations
            new(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, TypeSymbol.Bool),
            new(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, TypeSymbol.Bool),
            new(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Bool),
            new(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool)
        };

        public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, TypeSymbol leftType, TypeSymbol rightType)
        {
            if (syntaxKind == SyntaxKind.PlusToken)
            {
                if (leftType == TypeSymbol.String || rightType == TypeSymbol.String ||
                    leftType == TypeSymbol.Any || rightType == TypeSymbol.Any)
                {
                    return new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, leftType, rightType, TypeSymbol.String);
                }
            }

            if (syntaxKind == SyntaxKind.EqualsEqualsToken || syntaxKind == SyntaxKind.BangEqualsToken)
            {
                if (leftType == TypeSymbol.Any || rightType == TypeSymbol.Any || leftType == TypeSymbol.Vector)
                {
                    var kind = syntaxKind == SyntaxKind.EqualsEqualsToken ? BoundBinaryOperatorKind.Equals : BoundBinaryOperatorKind.NotEquals;
                    return new BoundBinaryOperator(syntaxKind, kind, leftType, rightType, TypeSymbol.Bool);
                }
            }

            for (int i = 0; i < _operators.Length; i++)
            {
                if (_operators[i].SyntaxKind == syntaxKind && _operators[i].LeftType == leftType && _operators[i].RightType == rightType)
                    return _operators[i];
            }
            return null;
        }
    }
}