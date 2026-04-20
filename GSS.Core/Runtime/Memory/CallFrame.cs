namespace GSS.Core.Runtime.Memory
{
	public readonly struct CallFrame
	{
		public readonly int ReturnInstructionPointer;
		public readonly int BaseRegisterIndex;

		public CallFrame(int returnInstructionPointer, int baseRegisterIndex)
		{
			ReturnInstructionPointer = returnInstructionPointer;
			BaseRegisterIndex = baseRegisterIndex;
		}
	}
}