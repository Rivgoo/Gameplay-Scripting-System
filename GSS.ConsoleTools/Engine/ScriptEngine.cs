using System.Diagnostics;
using GSS.ConsoleApp.Models;
using GSS.Core.ApiBinding.Registry;
using GSS.Core.Compiler.Diagnostics;
using GSS.Core.Runtime.Execution;
using GSS.Core.Runtime.Memory;
using GSS.Sharp.Compilation;

namespace GSS.ConsoleApp.Engine
{
	public sealed class ScriptEngine
	{
		private readonly GssCompiler _compiler;
		private readonly RuntimeScheduler _scheduler;
		private readonly AppEnvironment _environment;

		public ScriptEngine()
		{
			var builder = new ApiRegistryBuilder();
			builder.Register(new AppEnvironmentProfile());
			var registry = builder.Build();

			_compiler = new GssCompiler(registry);

			var pool = new ExecutionContextPool();
			_scheduler = new RuntimeScheduler(pool);
			_scheduler.OnScriptFaulted = HandleScriptFault;

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

			_scheduler.Enqueue(_environment, graph!);
			RunGameLoop();
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

		private void RunGameLoop()
		{
			var sw = Stopwatch.StartNew();
			float lastTime = 0f;

			while (_scheduler.IsRunning)
			{
				float currentTime = (float)sw.Elapsed.TotalSeconds;
				float deltaTime = currentTime - lastTime;
				lastTime = currentTime;

				_scheduler.Tick(deltaTime);

				Thread.Sleep(16); // Simulate ~60 FPS wait
			}

			sw.Stop();
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
			Console.WriteLine(ex.Message);
			Console.ResetColor();
		}
	}
}