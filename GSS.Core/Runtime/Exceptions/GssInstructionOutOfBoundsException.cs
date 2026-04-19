namespace GSS.Core.Runtime.Exceptions
{
	public sealed class GssInstructionOutOfBoundsException : GssRuntimeException
	{
		public GssInstructionOutOfBoundsException(int instructionPointer, int graphSize)
			: base($"Instruction Pointer ({instructionPointer}) went out of bounds. Graph size: {graphSize}.") { }
	}
}