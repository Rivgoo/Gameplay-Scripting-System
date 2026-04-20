using GSS.Core.ApiBinding.Models;
using GSS.Core.Compiler.Emission;

namespace GSS.Core.Runtime.Execution
{
	public sealed class ExecutableGraph
	{
		public Instruction[] Instructions { get; }
		public GssValue[] ConstantsPool { get; }
		public object[] MetadataPool { get; }
		public int RequiredRegisters { get; }

		public ExecutableGraph(Instruction[] instructions, GssValue[] constantsPool, object[] metadataPool, int requiredRegisters)
		{
			Instructions = instructions;
			ConstantsPool = constantsPool;
			MetadataPool = metadataPool;
			RequiredRegisters = requiredRegisters;
		}
	}
}