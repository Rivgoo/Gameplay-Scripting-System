namespace GSS.Kernel.Text
{
    public sealed class SourceText : ISourceText
    {
        public string RawText { get; }
        public int Length => RawText.Length;

        private readonly int[] _lineStarts;
        private int _lastQueriedPosition;
        private int _lastQueriedLineIndex;

        public SourceText(string text)
        {
            RawText = text;
            _lineStarts = ParseLineStarts(text);
            _lastQueriedPosition = 0;
            _lastQueriedLineIndex = 0;
        }

        public ReadOnlySpan<char> GetSpan(StringSegment segment)
        {
            if (segment.Length == 0) return ReadOnlySpan<char>.Empty;
            return RawText.AsSpan(segment.Start, segment.Length);
        }

        public int GetLineIndex(int position)
        {
            if (position == _lastQueriedPosition) return _lastQueriedLineIndex;

            if (position >= _lastQueriedPosition &&
                (_lastQueriedLineIndex == _lineStarts.Length - 1 || position < _lineStarts[_lastQueriedLineIndex + 1]))
            {
                _lastQueriedPosition = position; return _lastQueriedLineIndex;
            }

            int lower = 0;
            int upper = _lineStarts.Length - 1;

            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = _lineStarts[index];

                if (position == start) { UpdateCache(position, index); return index; }
                if (start > position) upper = index - 1;
                else lower = index + 1;
            }

            UpdateCache(position, lower - 1);
            return lower - 1;
        }

        public int GetCharacterOffset(int position)
        {
            int lineIndex = GetLineIndex(position);
            int lineStart = _lineStarts[lineIndex];
            return position - lineStart;
        }

        private void UpdateCache(int position, int lineIndex)
        {
            _lastQueriedPosition = position;
            _lastQueriedLineIndex = lineIndex;
        }

        private static int[] ParseLineStarts(string text)
        {
            var starts = new List<int>(128) { 0 };
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n') starts.Add(i + 1);
                else if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n') { starts.Add(i + 2); i++; }
            }
            return starts.ToArray();
        }
    }
}