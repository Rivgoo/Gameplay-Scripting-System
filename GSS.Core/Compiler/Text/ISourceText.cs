namespace GSS.Core.Compiler.Text
{
	public interface ISourceText
	{
		string RawText { get; }
		int Length { get; }
		int GetLineIndex(int position);
		int GetCharacterOffset(int position);
	}
}