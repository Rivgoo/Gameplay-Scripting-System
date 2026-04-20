namespace GSS.Core.Compiler.Syntax
{
	public interface IToken
	{
		int Type { get; }
		int Position { get; }
		int Length { get; }
		string? Text { get; }
		object? Value { get; }
	}
}
