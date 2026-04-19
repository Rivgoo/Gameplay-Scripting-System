namespace GSS.Core.Runtime.Exceptions
{
	public sealed class GssInfiniteLoopException : GssRuntimeException
	{
		public GssInfiniteLoopException(int instructionPointer, int maxSteps)
			: base($"Script exceeded maximum allowed execution steps ({maxSteps} per tick). Possible infinite loop detected at IP: {instructionPointer}.") { }
	}
}