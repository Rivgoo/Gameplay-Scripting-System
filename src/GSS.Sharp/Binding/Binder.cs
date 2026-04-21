using GSS.Kernel.Api.Abstractions;
using GSS.Kernel.Diagnostics;
using GSS.Kernel.Primitives;
using GSS.Kernel.Text;
using GSS.Sharp.Binding.Nodes;
using GSS.Sharp.Binding.Nodes.Expressions;
using GSS.Sharp.Binding.Nodes.Statements;
using GSS.Sharp.Binding.Symbols;
using GSS.Sharp.Diagnostics;
using GSS.Sharp.Emission;
using GSS.Sharp.Syntax;
using GSS.Sharp.Syntax.Nodes;
using GSS.Sharp.Syntax.Nodes.Expressions;
using GSS.Sharp.Syntax.Nodes.Statements;
using GSS.Sharp.Syntax.Nodes.Types;

namespace GSS.Sharp.Binding
{
    public sealed class Binder
    {
        private readonly ISourceText _source;
        private readonly DiagnosticBag _diagnostics;
        private readonly IClassMetadata _apiContext;
        private BoundScope _scope;
        public readonly RegisterAllocator Allocator;

        private readonly Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack = new();
        private int _labelCounter;

        public Binder(ISourceText source, DiagnosticBag diagnostics, IClassMetadata apiContext)
        {
            _source = source;
            _diagnostics = diagnostics;
            _apiContext = apiContext;
            _scope = new BoundScope(null);
            Allocator = new RegisterAllocator();
        }

        private BoundLabel GenerateLabel(string prefix) => new($"{prefix}_{++_labelCounter}");
        private TextLocation GetLocation(TextSpan span) => new(_source, span);

        public BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = new List<BoundStatement>();
            _scope = new BoundScope(_scope);

            for (int i = 0; i < syntax.Statements.Count; i++)
            {
                statements.Add(BindStatement(syntax.Statements[i]));
            }

            _scope = _scope.Parent!;
            return new BoundBlockStatement(statements);
        }

        private BoundStatement BindStatement(StatementSyntax syntax) => syntax.Kind switch
        {
            SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
            SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
            SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax),
            SyntaxKind.ImplicitVariableDeclaration => BindImplicitVariableDeclaration((ImplicitVariableDeclarationSyntax)syntax),
            SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
            SyntaxKind.WhileStatement => BindWhileStatement((WhileStatementSyntax)syntax),
            SyntaxKind.ForStatement => BindForStatement((ForStatementSyntax)syntax),
            SyntaxKind.ForEachStatement => BindForEachStatement((ForEachStatementSyntax)syntax),
            SyntaxKind.BreakStatement => BindBreakStatement((BreakStatementSyntax)syntax),
            SyntaxKind.ContinueStatement => BindContinueStatement((ContinueStatementSyntax)syntax),
            SyntaxKind.ReturnStatement => BindReturnStatement((ReturnStatementSyntax)syntax),
            SyntaxKind.ImportDirective => new BoundBlockStatement(new List<BoundStatement>()),
            _ => ReportUnsupportedStatement(syntax)
        };

        private BoundStatement ReportUnsupportedStatement(StatementSyntax syntax)
        {
            _diagnostics.Report(SemanticDiagnosticRules.FeatureNotSupported, GetLocation(syntax.Span), syntax.Kind.ToString());
            return new BoundBlockStatement(new List<BoundStatement>());
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            return new BoundExpressionStatement(BindExpression(syntax.Expression));
        }

        private BoundStatement BindBreakStatement(BreakStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.Report(SemanticDiagnosticRules.InvalidBreakContext, GetLocation(syntax.Span));
                return new BoundBlockStatement(new List<BoundStatement>());
            }
            return new BoundGotoStatement(_loopStack.Peek().BreakLabel);
        }

        private BoundStatement BindContinueStatement(ContinueStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.Report(SemanticDiagnosticRules.InvalidContinueContext, GetLocation(syntax.Span));
                return new BoundBlockStatement(new List<BoundStatement>());
            }
            return new BoundGotoStatement(_loopStack.Peek().ContinueLabel);
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var type = ResolveType(syntax.Type);
            var initializer = BindExpression(syntax.Initializer, type);

            int registerIndex = Allocator.Allocate();
            var variable = new VariableSymbol(syntax.IdentifierToken.Text, type, registerIndex, false, true);

            if (!_scope.TryDeclare(variable))
                _diagnostics.Report(SemanticDiagnosticRules.VariableAlreadyDeclared, syntax.IdentifierToken.Location, syntax.IdentifierToken.Text);

            return new BoundVariableDeclaration(variable, initializer);
        }

        private BoundStatement BindImplicitVariableDeclaration(ImplicitVariableDeclarationSyntax syntax)
        {
            var initializer = BindExpression(syntax.Initializer);

            int registerIndex = Allocator.Allocate();
            var variable = new VariableSymbol(syntax.IdentifierToken.Text, initializer.Type, registerIndex, false, true);

            if (!_scope.TryDeclare(variable))
                _diagnostics.Report(SemanticDiagnosticRules.VariableAlreadyDeclared, syntax.IdentifierToken.Location, syntax.IdentifierToken.Text);

            return new BoundVariableDeclaration(variable, initializer);
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseStatement == null ? null : BindStatement(syntax.ElseStatement);
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var breakLabel = GenerateLabel("break");
            var continueLabel = GenerateLabel("continue");

            _loopStack.Push((breakLabel, continueLabel));
            var body = BindStatement(syntax.Body);
            _loopStack.Pop();

            return new BoundWhileStatement(condition, body, breakLabel, continueLabel);
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var statements = new List<BoundStatement>();
            _scope = new BoundScope(_scope);

            if (syntax.Initializer != null) statements.Add(BindStatement(syntax.Initializer));

            var condition = syntax.Condition != null
                ? BindExpression(syntax.Condition, TypeSymbol.Bool)
                : new BoundLiteralExpression(true, TypeSymbol.Bool);

            var breakLabel = GenerateLabel("break");
            var continueLabel = GenerateLabel("continue");

            _loopStack.Push((breakLabel, continueLabel));
            var body = BindStatement(syntax.Body);
            _loopStack.Pop();

            var bodyBlockStatements = new List<BoundStatement> { body };

            if (syntax.Increment != null)
            {
                bodyBlockStatements.Add(new BoundLabelStatement(continueLabel));
                bodyBlockStatements.Add(new BoundExpressionStatement(BindExpression(syntax.Increment)));
                var loopEval = GenerateLabel("loop_eval");
                bodyBlockStatements.Add(new BoundGotoStatement(loopEval));

                statements.Add(new BoundWhileStatement(condition, new BoundBlockStatement(bodyBlockStatements), breakLabel, loopEval));
            }
            else
            {
                statements.Add(new BoundWhileStatement(condition, new BoundBlockStatement(bodyBlockStatements), breakLabel, continueLabel));
            }

            _scope = _scope.Parent!;
            return new BoundBlockStatement(statements);
        }

        private BoundStatement BindForEachStatement(ForEachStatementSyntax syntax)
        {
            if (syntax.CollectionExpression is RangeExpressionSyntax range)
            {
                var startExpr = BindExpression(range.StartExpression, TypeSymbol.Int);
                var endExpr = BindExpression(range.EndExpression, TypeSymbol.Int);

                _scope = new BoundScope(_scope);
                int reg = Allocator.Allocate();
                var variable = new VariableSymbol(syntax.IdentifierToken.Text, TypeSymbol.Int, reg, false, true);
                _scope.TryDeclare(variable);

                var init = new BoundVariableDeclaration(variable, startExpr);
                var condition = new BoundBinaryExpression(
                    new BoundVariableExpression(variable),
                    BoundBinaryOperator.Bind(SyntaxKind.LessToken, TypeSymbol.Int, TypeSymbol.Int)!,
                    endExpr);

                var breakLabel = GenerateLabel("break");
                var continueLabel = GenerateLabel("continue");

                _loopStack.Push((breakLabel, continueLabel));
                var body = BindStatement(syntax.Body);
                _loopStack.Pop();

                var increment = new BoundExpressionStatement(new BoundAssignmentExpression(variable,
                    new BoundBinaryExpression(
                        new BoundVariableExpression(variable),
                        BoundBinaryOperator.Bind(SyntaxKind.PlusToken, TypeSymbol.Int, TypeSymbol.Int)!,
                        new BoundLiteralExpression(1, TypeSymbol.Int)
                    )
                ));

                var bodyBlockStatements = new List<BoundStatement> { body, new BoundLabelStatement(continueLabel), increment };
                var loopEval = GenerateLabel("loop_eval");
                bodyBlockStatements.Add(new BoundGotoStatement(loopEval));

                var whileLoop = new BoundWhileStatement(condition, new BoundBlockStatement(bodyBlockStatements), breakLabel, loopEval);
                var block = new BoundBlockStatement(new List<BoundStatement> { init, whileLoop });

                _scope = _scope.Parent!;
                return block;
            }

            _diagnostics.Report(SemanticDiagnosticRules.FeatureNotSupported, GetLocation(syntax.CollectionExpression.Span), "ForEach over Objects");
            return new BoundBlockStatement(new List<BoundStatement>());
        }

        private BoundStatement BindReturnStatement(ReturnStatementSyntax syntax)
        {
            var expression = syntax.Expression == null ? null : BindExpression(syntax.Expression);
            return new BoundReturnStatement(expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType)
        {
            var result = BindExpression(syntax);
            if (targetType != TypeSymbol.Error && result.Type != TypeSymbol.Error && result.Type != targetType)
            {
                var conversion = Conversion.Classify(result.Type, targetType);
                if (!conversion.Exists)
                {
                    _diagnostics.Report(SemanticDiagnosticRules.TypeMismatch, GetLocation(syntax.Span), result.Type.Name, targetType.Name);
                    return result;
                }
                if (conversion.IsImplicit) return new BoundConversionExpression(targetType, result);
            }
            return result;
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax) => syntax.Kind switch
        {
            SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
            SyntaxKind.IdentifierExpression => BindIdentifierExpression((IdentifierExpressionSyntax)syntax),
            SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
            SyntaxKind.ElementAccessExpression => BindElementAccessExpression((ElementAccessExpressionSyntax)syntax),
            SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
            SyntaxKind.PostfixUnaryExpression => BindPostfixUnaryExpression((PostfixUnaryExpressionSyntax)syntax),
            SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
            SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)syntax),
            SyntaxKind.TernaryExpression => BindTernaryExpression((TernaryExpressionSyntax)syntax),

            SyntaxKind.MemberAccessExpression => ReportUnsupportedExpression(syntax, "Deep Member Access (obj.Prop)"),
            SyntaxKind.NullCoalescingExpression => ReportUnsupportedExpression(syntax, "Null Coalescing (??)"),
            SyntaxKind.RangeExpression => ReportUnsupportedExpression(syntax, "Standalone Range (..)"),
            SyntaxKind.NullConditionalAccessExpression => ReportUnsupportedExpression(syntax, "Null Conditional (?.)"),
            _ => ReportUnsupportedExpression(syntax, syntax.Kind.ToString())
        };

        private BoundExpression ReportUnsupportedExpression(ExpressionSyntax syntax, string feature)
        {
            _diagnostics.Report(SemanticDiagnosticRules.FeatureNotSupported, GetLocation(syntax.Span), feature);
            return new BoundLiteralExpression(0, TypeSymbol.Error);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var kind = syntax.LiteralToken.Kind;
            object? value = syntax.LiteralToken.Value;

            if (kind == SyntaxKind.TrueKeyword) value = true;
            else if (kind == SyntaxKind.FalseKeyword) value = false;
            else if (kind == SyntaxKind.NullKeyword) value = null;
            else value ??= 0;

            var type = kind switch
            {
                SyntaxKind.NumberToken => value is float ? TypeSymbol.Float : TypeSymbol.Int,
                SyntaxKind.StringToken => TypeSymbol.String,
                SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword => TypeSymbol.Bool,
                _ => TypeSymbol.Error
            };

            return new BoundLiteralExpression(value, type);
        }

        private BoundExpression BindIdentifierExpression(IdentifierExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            if (_scope.TryLookup(name, out var variable))
            {
                if (!variable.IsAssigned)
                {
                    _diagnostics.Report(SemanticDiagnosticRules.UninitializedVariable, syntax.IdentifierToken.Location, name);
                }
                return new BoundVariableExpression(variable);
            }

            if (_apiContext.TryGetProperty(name, out var prop))
            {
                return new BoundApiPropertyExpression(prop, ParseApiType(prop.Type));
            }

            _diagnostics.Report(SemanticDiagnosticRules.UndefinedVariable, syntax.IdentifierToken.Location, name);
            return new BoundLiteralExpression(0, TypeSymbol.Error);
        }

        private BoundExpression BindElementAccessExpression(ElementAccessExpressionSyntax syntax)
        {
            var collection = BindExpression(syntax.Expression);
            var index = BindExpression(syntax.IndexArgument, TypeSymbol.Int);
            return new BoundIndexExpression(collection, index, TypeSymbol.Any);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var boundRight = BindExpression(syntax.Right);

            if (syntax.Left is IdentifierExpressionSyntax id)
            {
                var name = id.IdentifierToken.Text;
                if (_scope.TryLookup(name, out var variable))
                {
                    if (variable.IsReadOnly)
                        _diagnostics.Report(SemanticDiagnosticRules.CannotAssignReadOnly, GetLocation(syntax.Left.Span), name);

                    variable.IsAssigned = true;
                    return new BoundAssignmentExpression(variable, BindExpression(syntax.Right, variable.Type));
                }

                if (_apiContext.TryGetProperty(name, out var prop))
                {
                    if (!prop.CanWrite)
                        _diagnostics.Report(SemanticDiagnosticRules.CannotAssignReadOnly, GetLocation(syntax.Left.Span), name);

                    var propType = ParseApiType(prop.Type);
                    return new BoundApiAssignmentExpression(prop, BindExpression(syntax.Right, propType));
                }
            }
            else if (syntax.Left is ElementAccessExpressionSyntax elementAccess)
            {
                var collection = BindExpression(elementAccess.Expression);
                var index = BindExpression(elementAccess.IndexArgument, TypeSymbol.Int);
                return new BoundIndexAssignmentExpression(collection, index, boundRight);
            }

            return boundRight;
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            if (syntax.OperatorToken.Kind == SyntaxKind.PlusPlusToken || syntax.OperatorToken.Kind == SyntaxKind.MinusMinusToken)
            {
                return HandleIncrementDecrement(syntax.Operand, syntax.OperatorToken);
            }

            var operand = BindExpression(syntax.Operand);
            var op = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);
            if (op == null)
            {
                _diagnostics.Report(SemanticDiagnosticRules.InvalidOperator, syntax.OperatorToken.Location, syntax.OperatorToken.Text, operand.Type.Name);
                return operand;
            }
            return new BoundUnaryExpression(op, operand);
        }

        private BoundExpression BindPostfixUnaryExpression(PostfixUnaryExpressionSyntax syntax)
        {
            return HandleIncrementDecrement(syntax.Operand, syntax.OperatorToken);
        }

        private BoundExpression HandleIncrementDecrement(ExpressionSyntax operandSyntax, SyntaxToken operatorToken)
        {
            var operand = BindExpression(operandSyntax);

            if (operandSyntax is IdentifierExpressionSyntax id)
            {
                var name = id.IdentifierToken.Text;
                if (_scope.TryLookup(name, out var variable))
                {
                    if (variable.IsReadOnly)
                        _diagnostics.Report(SemanticDiagnosticRules.CannotAssignReadOnly, GetLocation(operandSyntax.Span), name);

                    var one = new BoundLiteralExpression(1, variable.Type == TypeSymbol.Float ? TypeSymbol.Float : TypeSymbol.Int);
                    var tokenKind = operatorToken.Kind == SyntaxKind.PlusPlusToken ? SyntaxKind.PlusToken : SyntaxKind.MinusToken;
                    var op = BoundBinaryOperator.Bind(tokenKind, variable.Type, one.Type);

                    if (op == null)
                    {
                        _diagnostics.Report(SemanticDiagnosticRules.InvalidOperator, operatorToken.Location, operatorToken.Text, variable.Type.Name);
                        return operand;
                    }

                    var binary = new BoundBinaryExpression(operand, op, one);
                    return new BoundAssignmentExpression(variable, binary);
                }
            }

            _diagnostics.Report(SemanticDiagnosticRules.InvalidAssignmentTarget, operatorToken.Location, operatorToken.Text);
            return operand;
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var left = BindExpression(syntax.Left);
            var right = BindExpression(syntax.Right);
            if (left.Type == TypeSymbol.Error || right.Type == TypeSymbol.Error) return left;

            var op = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);
            if (op == null)
            {
                _diagnostics.Report(SemanticDiagnosticRules.InvalidOperator, syntax.OperatorToken.Location, syntax.OperatorToken.Text, left.Type.Name + " and " + right.Type.Name);
                return left;
            }
            return new BoundBinaryExpression(left, op, right);
        }

        private BoundExpression BindTernaryExpression(TernaryExpressionSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var trueExpr = BindExpression(syntax.TrueExpression);
            var falseExpr = BindExpression(syntax.FalseExpression, trueExpr.Type);
            return new BoundTernaryExpression(condition, trueExpr, falseExpr);
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            if (syntax.Identifier is not IdentifierExpressionSyntax idSyntax)
            {
                _diagnostics.Report(SemanticDiagnosticRules.UndefinedMethod, GetLocation(syntax.Identifier.Span), "?", _apiContext.Name);
                return new BoundLiteralExpression(0, TypeSymbol.Error);
            }

            var methodName = idSyntax.IdentifierToken.Text;
            var boundArgs = new List<BoundExpression>();

            for (int i = 0; i < syntax.Arguments.Count; i++)
            {
                boundArgs.Add(BindExpression(syntax.Arguments[i]));
            }

            if (_apiContext.TryGetMethod(methodName, boundArgs.Count, out var methodMeta))
            {
                var returnType = ParseApiType(methodMeta.ReturnType);
                return new BoundCallExpression(methodMeta, boundArgs, returnType);
            }

            _diagnostics.Report(SemanticDiagnosticRules.ArgumentCountMismatch, GetLocation(syntax.Identifier.Span), methodName, boundArgs.Count);
            return new BoundLiteralExpression(0, TypeSymbol.Error);
        }

        private TypeSymbol ResolveType(TypeSyntax syntax)
        {
            if (syntax is IdentifierNameSyntax id)
            {
                return id.IdentifierToken.Text switch
                {
                    "int" => TypeSymbol.Int,
                    "float" => TypeSymbol.Float,
                    "bool" => TypeSymbol.Bool,
                    "string" => TypeSymbol.String,
                    "vector" => TypeSymbol.Vector,
                    _ => TypeSymbol.Error
                };
            }
            return TypeSymbol.Error;
        }

        private TypeSymbol ParseApiType(VariantType type) => type switch
        {
            VariantType.Int => TypeSymbol.Int,
            VariantType.Float => TypeSymbol.Float,
            VariantType.Bool => TypeSymbol.Bool,
            VariantType.Vector => TypeSymbol.Vector,
            VariantType.Object => TypeSymbol.Any,
            VariantType.Null => TypeSymbol.Void,
            _ => TypeSymbol.Error
        };
    }
}