namespace GSS.Runtime.Exceptions
{
    public sealed class InstructionOutOfBoundsException : RuntimeException
    {
        public InstructionOutOfBoundsException(int instructionPointer, int graphSize)
            : base($"Instruction Pointer ({instructionPointer}) went out of bounds. Graph size: {graphSize}.") { }
    }
}