using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Text;
using GSS.Sharp.Binding.Nodes;
using GSS.Sharp.Binding.Nodes.Expressions;
using GSS.Sharp.Binding.Nodes.Statements;
using GSS.Sharp.Binding.Symbols;
using GSS.Sharp.Diagnostics;
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
		private int _maxAllocatedVariables;

		private readonly Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack = new();
		private int _labelCounter;

		public Binder(ISourceText source, DiagnosticBag diagnostics, IClassMetadata apiContext)
		{
			_source = source;
			_diagnostics = diagnostics;
			_apiContext = apiContext;
			_scope = new BoundScope(null, 0);
		}

		public int GetAllocatedVariableCount() => _maxAllocatedVariables;

		private BoundLabel GenerateLabel(string prefix) => new($"{prefix}_{++_labelCounter}");

		private TextLocation GetLocation(TextSpan span) => new(_source, span);

		public BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
		{
			var statements = new List<BoundStatement>();
			int currentOffset = _scope.GetAllocatedVariableCount();
			_scope = new BoundScope(_scope, currentOffset);

			for (int i = 0; i < syntax.Statements.Count; i++)
			{
				statements.Add(BindStatement(syntax.Statements[i]));
			}

			int allocatedInScope = _scope.GetAllocatedVariableCount();
			if (allocatedInScope > _maxAllocatedVariables) _maxAllocatedVariables = allocatedInScope;

			_scope = _scope.Parent!;
			return new BoundBlockStatement(statements);
		}

		private BoundStatement BindStatement(StatementSyntax syntax) => syntax.Kind switch
		{
			SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
			SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
			SyntaxKind.WaitStatement => BindWaitStatement((WaitStatementSyntax)syntax),
			SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax),
			SyntaxKind.ImplicitVariableDeclaration => BindImplicitVariableDeclaration((ImplicitVariableDeclarationSyntax)syntax),
			SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
			SyntaxKind.WhileStatement => BindWhileStatement((WhileStatementSyntax)syntax),
			SyntaxKind.ForStatement => BindForStatement((ForStatementSyntax)syntax),
			SyntaxKind.BreakStatement => BindBreakStatement((BreakStatementSyntax)syntax),
			SyntaxKind.ContinueStatement => BindContinueStatement((ContinueStatementSyntax)syntax),
			SyntaxKind.ReturnStatement => BindReturnStatement((ReturnStatementSyntax)syntax),
			_ => throw new Exception($"Unexpected syntax {syntax.Kind}")
		};

		private BoundStatement BindWaitStatement(WaitStatementSyntax syntax)
		{
			var duration = BindExpression(syntax.Duration, TypeSymbol.Float);
			return new BoundWaitStatement(duration);
		}

		private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
		{
			return new BoundExpressionStatement(BindExpression(syntax.Expression));
		}

		private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
		{
			var type = ResolveType(syntax.Type);
			var initializer = BindExpression(syntax.Initializer, type);

			int registerIndex = _scope.GetAllocatedVariableCount();
			var variable = new VariableSymbol(syntax.IdentifierToken.Text, type, registerIndex, false);

			if (!_scope.TryDeclare(variable))
				_diagnostics.Report(SemanticDiagnosticRules.VariableAlreadyDeclared, syntax.IdentifierToken.Location, syntax.IdentifierToken.Text);

			return new BoundVariableDeclaration(variable, initializer);
		}

		private BoundStatement BindImplicitVariableDeclaration(ImplicitVariableDeclarationSyntax syntax)
		{
			var initializer = BindExpression(syntax.Initializer);

			if (initializer.Type == TypeSymbol.Error || initializer.Type == TypeSymbol.Void)
			{
				_diagnostics.Report(SemanticDiagnosticRules.ImplicitVarWithoutInitializer, syntax.VarKeyword.Location);
				return new BoundVariableDeclaration(new VariableSymbol("?", TypeSymbol.Error, 0, false), initializer);
			}

			int registerIndex = _scope.GetAllocatedVariableCount();
			var variable = new VariableSymbol(syntax.IdentifierToken.Text, initializer.Type, registerIndex, false);

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
			int currentOffset = _scope.GetAllocatedVariableCount();
			_scope = new BoundScope(_scope, currentOffset);

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
				var explicitContinueForWhile = GenerateLabel("loop_eval");
				bodyBlockStatements.Add(new BoundGotoStatement(explicitContinueForWhile));

				statements.Add(new BoundWhileStatement(condition, new BoundBlockStatement(bodyBlockStatements), breakLabel, explicitContinueForWhile));
			}
			else
			{
				statements.Add(new BoundWhileStatement(condition, new BoundBlockStatement(bodyBlockStatements), breakLabel, continueLabel));
			}

			int allocatedInScope = _scope.GetAllocatedVariableCount();
			if (allocatedInScope > _maxAllocatedVariables) _maxAllocatedVariables = allocatedInScope;

			_scope = _scope.Parent!;
			return new BoundBlockStatement(statements);
		}

		private BoundStatement BindBreakStatement(BreakStatementSyntax syntax)
		{
			if (_loopStack.Count == 0)
			{
				_diagnostics.Report(SemanticDiagnosticRules.InvalidBreakContext, syntax.BreakKeyword.Location);
				return new BoundExpressionStatement(new BoundLiteralExpression(0, TypeSymbol.Error));
			}
			return new BoundGotoStatement(_loopStack.Peek().BreakLabel);
		}

		private BoundStatement BindContinueStatement(ContinueStatementSyntax syntax)
		{
			if (_loopStack.Count == 0)
			{
				_diagnostics.Report(SemanticDiagnosticRules.InvalidContinueContext, syntax.ContinueKeyword.Location);
				return new BoundExpressionStatement(new BoundLiteralExpression(0, TypeSymbol.Error));
			}
			return new BoundGotoStatement(_loopStack.Peek().ContinueLabel);
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
			SyntaxKind.ElementAccessExpression => BindElementAccessExpression((ElementAccessExpressionSyntax)syntax),
			SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
			SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
			SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
			SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)syntax),
			_ => throw new Exception($"Unexpected expression syntax {syntax.Kind}")
		};

		private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
		{
			var value = syntax.LiteralToken.Value ?? 0;
			var type = syntax.LiteralToken.Kind switch
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
			if (_scope.TryLookup(name, out var variable)) return new BoundVariableExpression(variable);

			if (_apiContext.TryRetrieveProperty(name, out var prop))
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
					return new BoundAssignmentExpression(variable, BindExpression(syntax.Right, variable.Type));
				}

				if (_apiContext.TryRetrieveProperty(name, out var prop))
				{
					if (!prop.CanWrite)
						_diagnostics.Report(SemanticDiagnosticRules.CannotAssignReadOnly, GetLocation(syntax.Left.Span), name);

					var propType = ParseApiType(prop.Type);
					return new BoundApiAssignmentExpression(prop, BindExpression(syntax.Right, propType));
				}

				_diagnostics.Report(SemanticDiagnosticRules.UndefinedVariable, GetLocation(syntax.Left.Span), name);
			}
			else if (syntax.Left is ElementAccessExpressionSyntax elementAccess)
			{
				var collection = BindExpression(elementAccess.Expression);
				var index = BindExpression(elementAccess.IndexArgument, TypeSymbol.Int);
				return new BoundIndexAssignmentExpression(collection, index, boundRight);
			}
			else
			{
				_diagnostics.Report(SemanticDiagnosticRules.InvalidAssignmentTarget, GetLocation(syntax.Left.Span), "assignment");
			}

			return boundRight;
		}

		private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
		{
			var operand = BindExpression(syntax.Operand);
			var op = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);
			if (op == null)
			{
				_diagnostics.Report(SemanticDiagnosticRules.InvalidOperator, syntax.OperatorToken.Location, syntax.OperatorToken.Text, operand.Type.Name);
				return operand;
			}
			return new BoundUnaryExpression(op, operand);
		}

		private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
		{
			var left = BindExpression(syntax.Left);
			var right = BindExpression(syntax.Right);
			if (left.Type == TypeSymbol.Error || right.Type == TypeSymbol.Error) return left;

			var op = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);
			if (op == null)
			{
				if (left.Type == TypeSymbol.String || right.Type == TypeSymbol.String)
				{
					if (left.Type != TypeSymbol.String) left = new BoundConversionExpression(TypeSymbol.String, left);
					if (right.Type != TypeSymbol.String) right = new BoundConversionExpression(TypeSymbol.String, right);
					op = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, TypeSymbol.String, TypeSymbol.String);
				}
				else if (left.Type == TypeSymbol.Int && right.Type == TypeSymbol.Float)
				{
					left = new BoundConversionExpression(TypeSymbol.Float, left);
					op = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, TypeSymbol.Float, TypeSymbol.Float);
				}
				else if (left.Type == TypeSymbol.Float && right.Type == TypeSymbol.Int)
				{
					right = new BoundConversionExpression(TypeSymbol.Float, right);
					op = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, TypeSymbol.Float, TypeSymbol.Float);
				}

				if (op == null)
				{
					_diagnostics.Report(SemanticDiagnosticRules.InvalidOperator, syntax.OperatorToken.Location, syntax.OperatorToken.Text, left.Type.Name + " and " + right.Type.Name);
					return left;
				}
			}
			return new BoundBinaryExpression(left, op, right);
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

			if (_apiContext.TryRetrieveMethod(methodName, boundArgs.Count, out var methodMeta))
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
					"Vector3" => TypeSymbol.Vector3,
					_ => TypeSymbol.Error
				};
			}
			return TypeSymbol.Error;
		}

		private TypeSymbol ParseApiType(GSS.Core.ApiBinding.Models.GssType type) => type switch
		{
			GSS.Core.ApiBinding.Models.GssType.Int => TypeSymbol.Int,
			GSS.Core.ApiBinding.Models.GssType.Float => TypeSymbol.Float,
			GSS.Core.ApiBinding.Models.GssType.Bool => TypeSymbol.Bool,
			GSS.Core.ApiBinding.Models.GssType.Object => TypeSymbol.Any,
			_ => TypeSymbol.Error
		};
	}
}