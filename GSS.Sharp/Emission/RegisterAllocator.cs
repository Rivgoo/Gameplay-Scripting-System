namespace GSS.Sharp.Emission
{
	public sealed class RegisterAllocator
	{
		private int _nextRegister;
		private int _maxAllocated;
		private readonly Stack<int> _freeRegisters = new();

		public RegisterAllocator()
		{
			_nextRegister = 0;
			_maxAllocated = 0;
		}

		public int Allocate()
		{
			if (_freeRegisters.Count > 0)
			{
				return _freeRegisters.Pop();
			}

			int reg = _nextRegister++;
			if (_nextRegister > _maxAllocated) _maxAllocated = _nextRegister;
			return reg;
		}

		public void Free(int register)
		{
			_freeRegisters.Push(register);
		}

		public int GetTotalRequiredRegisters() => _maxAllocated;
	}
}