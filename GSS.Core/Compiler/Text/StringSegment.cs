namespace GSS.Core.Compiler.Text
{
	public readonly struct StringSegment
	{
		public readonly int Start;
		public readonly int Length;

		public StringSegment(int start, int length)
		{
			Start = start;
			Length = length;
		}
	}
}