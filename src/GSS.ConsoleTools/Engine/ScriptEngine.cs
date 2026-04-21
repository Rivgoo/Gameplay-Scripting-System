using GSS.ConsoleTools.Models;
using GSS.Kernel.Api.Registry;
using GSS.Kernel.Diagnostics;
using GSS.Runtime.Generated;
using GSS.Runtime.Memory;
using GSS.Runtime.Scheduling;
using GSS.Sharp.Compilation;

namespace GSS.ConsoleTools.Engine
{
	public sealed class ScriptEngine
	{
		private readonly Compiler _compiler;
		private readonly ExecutionScheduler _scheduler;
		private readonly AppEnvironment _environment;

		public ScriptEngine()
		{
			var registry = new ApiRegistryBuilder()
				.RegisterAllProfiles()
				.Build();

			_compiler = new Compiler(registry);

			var pool = new ContextPool(16);
			_scheduler = new ExecutionScheduler(pool)
			{
				OnFaulted = HandleScriptFault
			};

			_environment = new AppEnvironment();
		}

		public void ExecuteSource(string sourceCode)
		{
			var graph = _compiler.Compile(sourceCode, "AppEnvironment", out var diagnostics);

			if (diagnostics.HasErrors)
			{
				PrintDiagnostics(diagnostics);
				return;
			}

			_scheduler.Enqueue(graph!, _environment);

			_scheduler.Tick();
		}

		public void ExecuteFile(string filepath)
		{
			if (!File.Exists(filepath))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Error: File '{filepath}' not found.");
				Console.ResetColor();
				return;
			}

			string sourceCode = File.ReadAllText(filepath);
			ExecuteSource(sourceCode);
		}

		private void PrintDiagnostics(DiagnosticBag bag)
		{
			foreach (var diag in bag.Diagnostics)
			{
				Console.ForegroundColor = diag.Descriptor.Severity == DiagnosticSeverity.Error
					? ConsoleColor.Red
					: ConsoleColor.Yellow;

				Console.WriteLine(diag.ToString());
			}
			Console.ResetColor();
		}

		private void HandleScriptFault(Exception ex, RuntimeExecutionContext ctx)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine($"[Runtime Panic] IP: {ctx.InstructionPointer}");
			Console.WriteLine($"Message: {ex.Message}");
			if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
			Console.ResetColor();
		}
	}
}