namespace GSS.Kernel.Text
{
    public interface ISourceText
    {
        string RawText { get; }
        int Length { get; }
        int GetLineIndex(int position);
        int GetCharacterOffset(int position);
    }
}