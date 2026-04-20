using GSS.Core.ApiBinding.Models;

namespace GSS.Core.ApiBinding.Abstractions
{
	public interface IMethodMetadata
	{
		string Name { get; }
		int ArgumentCount { get; }
		GssType ReturnType { get; }
		GssValue Invoke(object instance, ReadOnlySpan<GssValue> args);
	}
}