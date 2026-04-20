using GSS.ConsoleApp.Engine;

namespace GSS.ConsoleApp
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Console.Title = "Gameplay Scripting System - REPL";
			Console.WriteLine("========================================");
			Console.WriteLine(" GSS Console Runner Active");
			Console.WriteLine(" Type 'help' for commands, 'exit' to quit.");
			Console.WriteLine("========================================\n");

			var engine = new ScriptEngine();

			while (true)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("gss> ");
				Console.ResetColor();

				string? input = Console.ReadLine()?.Trim();
				if (string.IsNullOrEmpty(input)) continue;

				if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
					input.Equals("quit", StringComparison.OrdinalIgnoreCase))
				{
					break;
				}

				if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
				{
					PrintHelp();
					continue;
				}

				if(input.Equals("clear", StringComparison.OrdinalIgnoreCase))
				{
					Console.Clear();
					continue;
				}

				if(input.Equals("cls", StringComparison.OrdinalIgnoreCase))
				{
					Console.Clear();
					continue;
				}

				if (input.StartsWith("run ", StringComparison.OrdinalIgnoreCase))
				{
					string filepath = input.Substring(4).Trim();
					engine.ExecuteFile(filepath);
					continue;
				}

				if (input.StartsWith("eval ", StringComparison.OrdinalIgnoreCase))
				{
					string code = input.Substring(5).Trim();
					engine.ExecuteSource(code);
					continue;
				}

				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Unknown command. Type 'help' for available commands.");
				Console.ResetColor();
			}
		}

		private static void PrintHelp()
		{
			Console.WriteLine("Commands:");
			Console.WriteLine("  run <filepath>   - Compiles and executes a .gss file.");
			Console.WriteLine("  eval <code>      - Compiles and executes inline GSS code.");
			Console.WriteLine("  help             - Displays this message.");
			Console.WriteLine("  clear/cls        - Clears the console.");
			Console.WriteLine("  exit/quit        - Closes the application.");
			Console.WriteLine("\nAvailable API for 'eval':");
			Console.WriteLine("  Properties: PlayerName (string), PlayerHealth (float)");
			Console.WriteLine("  Methods: Print(string), Heal(float), Damage(float), GetRandom() -> float");
			Console.WriteLine("           PrintInt(int), PrintFloat(float), PrintBool(bool)");
			Console.WriteLine("\nExample:");
			Console.WriteLine("  eval var a = 10; if (a > 5) { Print(\"Success!\"); }");
		}
	}
}