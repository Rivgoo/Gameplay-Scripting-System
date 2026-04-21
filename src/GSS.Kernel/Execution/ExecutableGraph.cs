using GSS.Kernel.Instructions;
using GSS.Kernel.Primitives;

namespace GSS.Kernel.Execution
{
    public sealed class ExecutableGraph
    {
        public Instruction[] Instructions { get; }
        public Variant[] Constants { get; }
        public object[] Metadata { get; }
        public int RequiredRegisters { get; }

        public ExecutableGraph(Instruction[] instructions, Variant[] constants, object[] metadata, int requiredRegisters)
        {
            Instructions = instructions;
            Constants = constants;
            Metadata = metadata;
            RequiredRegisters = requiredRegisters;
        }
    }
}