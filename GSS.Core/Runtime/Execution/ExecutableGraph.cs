using GSS.Core.Runtime.Instructions.Abstractions;

namespace GSS.Core.Runtime.Execution
{
	public sealed class ExecutableGraph
	{
		public IInstruction[] Instructions { get; }
		public int VariableCount { get; }
		public int MaxArgumentCount { get; }

		public ExecutableGraph(IInstruction[] instructions, int variableCount, int maxArgumentCount)
		{
			Instructions = instructions;
			VariableCount = variableCount;
			MaxArgumentCount = maxArgumentCount;
		}
	}
}