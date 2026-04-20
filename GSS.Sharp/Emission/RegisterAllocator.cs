namespace GSS.Sharp.Emission
{
	internal sealed class RegisterAllocator
	{
		private readonly int _variableCount;
		private int _tempStart;
		private int _maxAllocated;
		private readonly Stack<int> _freeTemps = new();

		public RegisterAllocator(int variableCount)
		{
			_variableCount = variableCount;
			_tempStart = variableCount;
			_maxAllocated = variableCount;
		}

		public int AllocateTemp()
		{
			if (_freeTemps.Count > 0) return _freeTemps.Pop();

			int reg = _tempStart++;
			if (_tempStart > _maxAllocated) _maxAllocated = _tempStart;
			return reg;
		}

		public void FreeTemp(int reg)
		{
			if (reg >= _variableCount) _freeTemps.Push(reg);
		}

		public int GetTotalRequiredRegisters() => _maxAllocated;
	}
}
