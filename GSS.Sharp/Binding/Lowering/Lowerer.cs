using GSS.Sharp.Binding.Nodes;
using GSS.Sharp.Binding.Nodes.Statements;

namespace GSS.Sharp.Lowering
{
	public sealed class Lowerer
	{
		private int _labelCounter;

		private Lowerer() { }

		private BoundLabel GenerateLabel(string prefix) => new($"{prefix}_{++_labelCounter}");

		public static BoundBlockStatement Lower(BoundStatement statement)
		{
			var lowerer = new Lowerer();
			var result = lowerer.RewriteStatement(statement);
			return Flatten(result);
		}

		private BoundStatement RewriteStatement(BoundStatement node) => node.Kind switch
		{
			BoundNodeKind.IfStatement => RewriteIfStatement((BoundIfStatement)node),
			BoundNodeKind.WhileStatement => RewriteWhileStatement((BoundWhileStatement)node),
			BoundNodeKind.BlockStatement => RewriteBlockStatement((BoundBlockStatement)node),
			_ => node
		};

		private BoundStatement RewriteIfStatement(BoundIfStatement node)
		{
			var endLabel = GenerateLabel("endif");
			var elseLabel = node.ElseStatement != null ? GenerateLabel("else") : endLabel;

			var statements = new List<BoundStatement>
			{
				new BoundConditionalGotoStatement(elseLabel, node.Condition, jumpIfTrue: false),
				RewriteStatement(node.ThenStatement)
			};

			if (node.ElseStatement != null)
			{
				statements.Add(new BoundGotoStatement(endLabel));
				statements.Add(new BoundLabelStatement(elseLabel));
				statements.Add(RewriteStatement(node.ElseStatement));
			}

			statements.Add(new BoundLabelStatement(endLabel));
			return new BoundBlockStatement(statements);
		}

		private BoundStatement RewriteWhileStatement(BoundWhileStatement node)
		{
			var bodyLabel = GenerateLabel("while_body");

			var statements = new List<BoundStatement>
			{
				new BoundGotoStatement(node.ContinueLabel),
				new BoundLabelStatement(bodyLabel),
				RewriteStatement(node.Body),
				new BoundLabelStatement(node.ContinueLabel),
				new BoundConditionalGotoStatement(bodyLabel, node.Condition, jumpIfTrue: true),
				new BoundLabelStatement(node.BreakLabel)
			};

			return new BoundBlockStatement(statements);
		}

		private BoundStatement RewriteBlockStatement(BoundBlockStatement node)
		{
			var statements = new List<BoundStatement>();
			foreach (var statement in node.Statements)
			{
				statements.Add(RewriteStatement(statement));
			}
			return new BoundBlockStatement(statements);
		}

		private static BoundBlockStatement Flatten(BoundStatement statement)
		{
			var statements = new List<BoundStatement>();
			var stack = new Stack<BoundStatement>();
			stack.Push(statement);

			while (stack.Count > 0)
			{
				var current = stack.Pop();
				if (current is BoundBlockStatement block)
				{
					for (int i = block.Statements.Count - 1; i >= 0; i--) stack.Push(block.Statements[i]);
				}
				else
				{
					statements.Add(current);
				}
			}

			return new BoundBlockStatement(statements);
		}
	}
}