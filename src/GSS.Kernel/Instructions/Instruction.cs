using System.Runtime.InteropServices;

namespace GSS.Kernel.Instructions
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
    public readonly struct Instruction
    {
        public readonly OpCode Code;
        public readonly int Op1;
        public readonly int Op2;
        public readonly int Op3;

        public Instruction(OpCode code, int op1 = 0, int op2 = 0, int op3 = 0)
        {
            Code = code;
            Op1 = op1;
            Op2 = op2;
            Op3 = op3;
        }
    }
}