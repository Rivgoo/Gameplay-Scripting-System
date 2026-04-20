using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Exceptions;
using GSS.Core.Compiler.Syntax;
using GSS.Core.Compiler.Text;
using GSS.Sharp.Parsing.Exceptions;
using GSS.Sharp.Syntax;
using GSS.Sharp.Syntax.Nodes;
using GSS.Sharp.Syntax.Nodes.Expressions;
using GSS.Sharp.Syntax.Nodes.Statements;
using GSS.Sharp.Syntax.Nodes.Types;

namespace GSS.Sharp.Parsing
{
	public sealed class Parser : IParser
	{
		public DiagnosticBag Diagnostics { get; } = new();

		private readonly ISourceText _source;
		private readonly TokenBuffer _buffer;
		private int _recursionDepth;
		private const int MaxRecursionDepth = 500;

		public Parser(ISourceText source)
		{
			_source = source;
			var lexer = new Lexer(source);
			_buffer = new TokenBuffer(lexer);
			Diagnostics.AddRange(lexer.Diagnostics);
		}

		public IAstNode Parse()
		{
			var statements = new List<StatementSyntax>();
			while (Current().Kind != SyntaxKind.EndOfFileToken)
			{
				statements.Add(ParseStatement());
			}

			var eof = ExpectToken(SyntaxKind.EndOfFileToken);
			return new BlockStatementSyntax(new SyntaxToken(SyntaxKind.OpenBraceToken, 0, 0, eof.Source), statements, eof);
		}

		private SyntaxToken Current() => _buffer.Peek(0);
		private SyntaxToken Lookahead() => _buffer.Peek(1);
		private SyntaxToken NextToken() => _buffer.Consume();

		private TextLocation GetLocation(TextSpan span) => new(_source, span);

		private string GetSourceText(TextSpan span) => _source.RawText.Substring(span.Start, span.Length);

		private SyntaxToken ExpectToken(SyntaxKind kind)
		{
			if (Current().Kind == kind) return NextToken();

			Diagnostics.Report(DiagnosticRules.SynMissingToken, Current().Location, kind);
			return new SyntaxToken(kind, Current().Position, 0, Current().Source);
		}

		private void EnsureRecursionLimit()
		{
			if (++_recursionDepth > MaxRecursionDepth)
			{
				Diagnostics.Report(DiagnosticRules.SynRecursionLimit, Current().Location);
				throw new GssCompilationCanceledException("Parser recursion depth exceeded.");
			}
		}

		private void Synchronize()
		{
			int startPos = Current().Position;
			NextToken();
			int bracesCount = 0;

			while (Current().Kind != SyntaxKind.EndOfFileToken)
			{
				if (Current().Kind == SyntaxKind.OpenBraceToken) bracesCount++;
				if (Current().Kind == SyntaxKind.CloseBraceToken)
				{
					if (bracesCount == 0) break;
					bracesCount--;
				}

				if (Current().Kind == SyntaxKind.SemicolonToken && bracesCount == 0)
				{
					NextToken();
					break;
				}

				if (bracesCount == 0 && IsStatementBoundary(Current().Kind)) break;
				NextToken();
			}

			var skippedSpan = new TextSpan(startPos, Current().Position - startPos);
			Diagnostics.Report(DiagnosticRules.SynSkippedTokens, GetLocation(skippedSpan));
		}

		private bool IsStatementBoundary(SyntaxKind kind)
		{
			return kind is SyntaxKind.ImportKeyword or SyntaxKind.IfKeyword or SyntaxKind.WhileKeyword or
				   SyntaxKind.ForKeyword or SyntaxKind.ReturnKeyword or SyntaxKind.VarKeyword;
		}

		private StatementSyntax ParseStatement()
		{
			try
			{
				if (Current().Kind == SyntaxKind.VarKeyword)
					return ParseImplicitVariableDeclaration();

				if (Current().Kind == SyntaxKind.IdentifierToken && Lookahead().Kind == SyntaxKind.IdentifierToken)
					return ParseVariableDeclaration();

				return Current().Kind switch
				{
					SyntaxKind.ImportKeyword => ParseImportDirective(),
					SyntaxKind.OpenBraceToken => ParseBlockStatement(),
					SyntaxKind.IfKeyword => ParseIfStatement(),
					SyntaxKind.WhileKeyword => ParseWhileStatement(),
					SyntaxKind.ForKeyword => ParseForStatement(),
					SyntaxKind.BreakKeyword => ParseBreakStatement(),
					SyntaxKind.ContinueKeyword => ParseContinueStatement(),
					SyntaxKind.ReturnKeyword => ParseReturnStatement(),
					_ => ParseExpressionStatement()
				};
			}
			catch (GssParseException)
			{
				Synchronize();
				return new ExpressionStatementSyntax(new LiteralExpressionSyntax(new SyntaxToken(SyntaxKind.NullKeyword, Current().Position, 0, Current().Source)), new SyntaxToken());
			}
		}

		private TypeSyntax ParseType()
		{
			var identifier = ExpectToken(SyntaxKind.IdentifierToken);
			if (Current().Kind == SyntaxKind.LessToken)
			{
				var less = NextToken();
				var typeArgs = new List<TypeSyntax> { ParseType() };

				while (Current().Kind == SyntaxKind.CommaToken)
				{
					NextToken();
					typeArgs.Add(ParseType());
				}

				var greater = ExpectToken(SyntaxKind.GreaterToken);
				return new GenericNameSyntax(identifier, less, typeArgs, greater);
			}
			return new IdentifierNameSyntax(identifier);
		}

		private VariableDeclarationSyntax ParseVariableDeclaration()
		{
			var type = ParseType();
			var identifier = ExpectToken(SyntaxKind.IdentifierToken);
			var equals = ExpectToken(SyntaxKind.EqualsToken);
			var initializer = ParseExpression(0);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new VariableDeclarationSyntax(type, identifier, equals, initializer, semicolon);
		}

		private ImplicitVariableDeclarationSyntax ParseImplicitVariableDeclaration()
		{
			var keyword = ExpectToken(SyntaxKind.VarKeyword);
			var identifier = ExpectToken(SyntaxKind.IdentifierToken);
			var equals = ExpectToken(SyntaxKind.EqualsToken);
			var initializer = ParseExpression(0);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new ImplicitVariableDeclarationSyntax(keyword, identifier, equals, initializer, semicolon);
		}

		private BlockStatementSyntax ParseBlockStatement()
		{
			var open = ExpectToken(SyntaxKind.OpenBraceToken);
			var statements = new List<StatementSyntax>();

			while (Current().Kind != SyntaxKind.EndOfFileToken && Current().Kind != SyntaxKind.CloseBraceToken)
			{
				statements.Add(ParseStatement());
			}

			var close = ExpectToken(SyntaxKind.CloseBraceToken);
			return new BlockStatementSyntax(open, statements, close);
		}

		private IfStatementSyntax ParseIfStatement()
		{
			var keyword = ExpectToken(SyntaxKind.IfKeyword);
			ExpectToken(SyntaxKind.OpenParenToken);
			var condition = ParseExpression(0);
			ExpectToken(SyntaxKind.CloseParenToken);
			var thenStmt = ParseStatement();

			StatementSyntax? elseStmt = null;
			if (Current().Kind == SyntaxKind.ElseKeyword)
			{
				NextToken();
				elseStmt = ParseStatement();
			}

			return new IfStatementSyntax(keyword, condition, thenStmt, elseStmt);
		}

		private WhileStatementSyntax ParseWhileStatement()
		{
			var keyword = ExpectToken(SyntaxKind.WhileKeyword);
			ExpectToken(SyntaxKind.OpenParenToken);
			var condition = ParseExpression(0);
			ExpectToken(SyntaxKind.CloseParenToken);
			var body = ParseStatement();
			return new WhileStatementSyntax(keyword, condition, body);
		}

		private StatementSyntax ParseForStatement()
		{
			var keyword = ExpectToken(SyntaxKind.ForKeyword);
			var openParen = ExpectToken(SyntaxKind.OpenParenToken);

			if (Current().Kind == SyntaxKind.VarKeyword && Lookahead().Kind == SyntaxKind.IdentifierToken && _buffer.Peek(2).Kind == SyntaxKind.InKeyword)
				return ParseForEachStatement(keyword, openParen);

			StatementSyntax? initializer = null;
			if (Current().Kind != SyntaxKind.SemicolonToken)
			{
				if (Current().Kind == SyntaxKind.VarKeyword) initializer = ParseImplicitVariableDeclaration();
				else if (Current().Kind == SyntaxKind.IdentifierToken && Lookahead().Kind == SyntaxKind.IdentifierToken) initializer = ParseVariableDeclaration();
				else initializer = ParseExpressionStatement();
			}
			else ExpectToken(SyntaxKind.SemicolonToken);

			ExpressionSyntax? condition = null;
			if (Current().Kind != SyntaxKind.SemicolonToken) condition = ParseExpression(0);
			ExpectToken(SyntaxKind.SemicolonToken);

			ExpressionSyntax? increment = null;
			if (Current().Kind != SyntaxKind.CloseParenToken) increment = ParseExpression(0);
			ExpectToken(SyntaxKind.CloseParenToken);

			var body = ParseStatement();
			return new ForStatementSyntax(keyword, initializer, condition, increment, body);
		}

		private ForEachStatementSyntax ParseForEachStatement(SyntaxToken forKeyword, SyntaxToken openParenToken)
		{
			var varKeyword = ExpectToken(SyntaxKind.VarKeyword);
			var identifier = ExpectToken(SyntaxKind.IdentifierToken);
			var inKeyword = ExpectToken(SyntaxKind.InKeyword);
			var collection = ParseExpression(0);
			var closeParen = ExpectToken(SyntaxKind.CloseParenToken);
			var body = ParseStatement();

			return new ForEachStatementSyntax(forKeyword, openParenToken, varKeyword, identifier, inKeyword, collection, closeParen, body);
		}

		private ReturnStatementSyntax ParseReturnStatement()
		{
			var keyword = ExpectToken(SyntaxKind.ReturnKeyword);
			ExpressionSyntax? expr = null;
			if (Current().Kind != SyntaxKind.SemicolonToken) expr = ParseExpression(0);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new ReturnStatementSyntax(keyword, expr, semicolon);
		}

		private BreakStatementSyntax ParseBreakStatement()
		{
			var keyword = ExpectToken(SyntaxKind.BreakKeyword);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new BreakStatementSyntax(keyword, semicolon);
		}

		private ContinueStatementSyntax ParseContinueStatement()
		{
			var keyword = ExpectToken(SyntaxKind.ContinueKeyword);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new ContinueStatementSyntax(keyword, semicolon);
		}

		private ExpressionStatementSyntax ParseExpressionStatement()
		{
			var expr = ParseExpression(0);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new ExpressionStatementSyntax(expr, semicolon);
		}

		private ImportDirectiveSyntax ParseImportDirective()
		{
			var keyword = ExpectToken(SyntaxKind.ImportKeyword);
			var identifier = ExpectToken(SyntaxKind.IdentifierToken);
			var semicolon = ExpectToken(SyntaxKind.SemicolonToken);
			return new ImportDirectiveSyntax(keyword, identifier, semicolon);
		}

		private ExpressionSyntax ParseExpression(int parentPrecedence)
		{
			EnsureRecursionLimit();
			ExpressionSyntax left;

			int unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(Current().Kind);
			if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
			{
				var operatorToken = NextToken();
				var operand = ParseExpression(unaryPrecedence);
				left = new UnaryExpressionSyntax(operatorToken, operand);
			}
			else
			{
				left = ParsePrimaryExpression();
			}

			while (true)
			{
				if (Current().Kind == SyntaxKind.QuestionDotToken)
				{
					var opToken = NextToken();
					var identifier = ExpectToken(SyntaxKind.IdentifierToken);
					left = new NullConditionalAccessExpressionSyntax(left, opToken, identifier);
					continue;
				}

				if (Current().Kind == SyntaxKind.DotToken)
				{
					var dotToken = NextToken();
					var identifier = ExpectToken(SyntaxKind.IdentifierToken);
					left = new MemberAccessExpressionSyntax(left, dotToken, identifier);
					continue;
				}

				if (Current().Kind == SyntaxKind.PlusPlusToken || Current().Kind == SyntaxKind.MinusMinusToken)
				{
					if (!left.IsAssignable)
					{
						Diagnostics.Report(DiagnosticRules.SynInvalidPostfix, GetLocation(left.Span), GetSourceText(left.Span));
					}

					left = new PostfixUnaryExpressionSyntax(left, NextToken());
					continue;
				}

				if (Current().Kind == SyntaxKind.DotDotToken)
				{
					var dotDotToken = NextToken();
					var endExpr = ParseExpression(0);
					left = new RangeExpressionSyntax(left, dotDotToken, endExpr);
					continue;
				}

				if (Current().Kind == SyntaxKind.OpenParenToken)
				{
					left = ParseCallExpression(left);
					continue;
				}

				int binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(Current().Kind);
				bool isAssignment = SyntaxFacts.IsAssignmentOperator(Current().Kind);
				bool isTernary = Current().Kind == SyntaxKind.QuestionToken;

				if (binaryPrecedence == 0 && !isAssignment) break;
				if (binaryPrecedence < parentPrecedence && !isAssignment) break;

				int nextPrecedence = isAssignment || isTernary ? 0 : binaryPrecedence + 1;

				if (isTernary)
				{
					left = ParseTernaryExpression(left);
					continue;
				}

				var operatorToken = NextToken();
				var right = ParseExpression(nextPrecedence);

				if (isAssignment)
				{
					if (!left.IsAssignable)
					{
						Diagnostics.Report(DiagnosticRules.SynInvalidAssignment, GetLocation(left.Span), GetSourceText(left.Span));
					}

					left = new AssignmentExpressionSyntax(left, operatorToken, right);
				}
				else if (operatorToken.Kind == SyntaxKind.QuestionQuestionToken)
				{
					left = new NullCoalescingExpressionSyntax(left, operatorToken, right);
				}
				else
				{
					left = new BinaryExpressionSyntax(left, operatorToken, right);
				}
			}

			_recursionDepth--;
			return left;
		}

		private ExpressionSyntax ParsePrimaryExpression()
		{
			switch (Current().Kind)
			{
				case SyntaxKind.OpenParenToken:
					var openParen = NextToken();
					var expression = ParseExpression(0);
					ExpectToken(SyntaxKind.CloseParenToken);
					return expression;

				case SyntaxKind.TrueKeyword:
				case SyntaxKind.FalseKeyword:
				case SyntaxKind.NullKeyword:
				case SyntaxKind.StringToken:
				case SyntaxKind.NumberToken:
					return new LiteralExpressionSyntax(NextToken());

				case SyntaxKind.IdentifierToken:
					return new IdentifierExpressionSyntax(NextToken());

				default:
					var badToken = NextToken();
					Diagnostics.Report(DiagnosticRules.SynExpectedExpression, badToken.Location, badToken.Kind);
					throw new GssParseException($"Expected valid primary expression but found {badToken.Kind}.");
			}
		}

		private ExpressionSyntax ParseTernaryExpression(ExpressionSyntax condition)
		{
			var questionToken = ExpectToken(SyntaxKind.QuestionToken);
			var trueExpr = ParseExpression(0);
			var colonToken = ExpectToken(SyntaxKind.ColonToken);
			var falseExpr = ParseExpression(0);
			return new TernaryExpressionSyntax(condition, questionToken, trueExpr, colonToken, falseExpr);
		}

		private CallExpressionSyntax ParseCallExpression(ExpressionSyntax identifier)
		{
			var openParen = ExpectToken(SyntaxKind.OpenParenToken);
			var arguments = new List<ExpressionSyntax>();

			if (Current().Kind != SyntaxKind.CloseParenToken)
			{
				while (true)
				{
					arguments.Add(ParseExpression(0));
					if (Current().Kind == SyntaxKind.CommaToken) NextToken();
					else break;
				}
			}

			var closeParen = ExpectToken(SyntaxKind.CloseParenToken);
			return new CallExpressionSyntax(identifier, openParen, arguments, closeParen);
		}
	}
}