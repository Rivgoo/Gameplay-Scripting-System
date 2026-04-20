using GSS.Core.ApiBinding.Abstractions;
using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Compiler.Text;
using GSS.Core.Runtime.Execution;
using GSS.Sharp.Binding;
using GSS.Sharp.Emission;
using GSS.Sharp.Lowering;
using GSS.Sharp.Parsing;
using GSS.Sharp.Syntax.Nodes.Statements;

namespace GSS.Sharp.Compilation
{
	public sealed class GssCompiler
	{
		private readonly IApiRegistry _registry;

		public GssCompiler(IApiRegistry registry)
		{
			_registry = registry;
		}

		public ExecutableGraph? Compile(string sourceCode, string targetApiProfile, out DiagnosticBag diagnostics)
		{
			var source = new SourceText(sourceCode);
			var parser = new Parser(source);
			var ast = (BlockStatementSyntax)parser.Parse();

			diagnostics = parser.Diagnostics;
			if (diagnostics.HasErrors) return null;

			var classMetadata = _registry.RetrieveClass(targetApiProfile);
			var binder = new Binder(source, diagnostics, classMetadata);
			var boundTree = binder.BindBlockStatement(ast);

			if (diagnostics.HasErrors) return null;

			var loweredTree = Lowerer.Lower(boundTree);

			var emitter = new BytecodeEmitter();
			var graph = emitter.Emit(loweredTree, binder.Allocator);

			return graph;
		}
	}
}