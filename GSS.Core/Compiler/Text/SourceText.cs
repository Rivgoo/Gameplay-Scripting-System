namespace GSS.Core.Compiler.Text
{
	public sealed class SourceText : ISourceText
	{
		public string RawText { get; }
		public int Length => RawText.Length;

		private readonly int[] _lineStarts;

		public SourceText(string text)
		{
			RawText = text;
			_lineStarts = ParseLineStarts(text);
		}

		public int GetLineIndex(int position)
		{
			int lower = 0;
			int upper = _lineStarts.Length - 1;

			while (lower <= upper)
			{
				int index = lower + (upper - lower) / 2;
				int start = _lineStarts[index];

				if (position == start) return index;
				if (start > position) upper = index - 1;
				else lower = index + 1;
			}

			return lower - 1;
		}

		public int GetCharacterOffset(int position)
		{
			int lineIndex = GetLineIndex(position);
			int lineStart = _lineStarts[lineIndex];
			return position - lineStart;
		}

		private static int[] ParseLineStarts(string text)
		{
			var starts = new List<int> { 0 };
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
				{
					starts.Add(i + 1);
				}
				else if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
				{
					starts.Add(i + 2);
					i++;
				}
			}
			return starts.ToArray();
		}
	}
}