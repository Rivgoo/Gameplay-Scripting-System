namespace GSS.Runtime.Exceptions
{
	public sealed class ExecutionLimitException : RuntimeException
    {
        public ExecutionLimitException(int instructionsExecuted) 
            : base($"Script execution halted. Exceeded maximum instruction limit per tick ({instructionsExecuted}). Possible infinite loop.") { }
    }
}