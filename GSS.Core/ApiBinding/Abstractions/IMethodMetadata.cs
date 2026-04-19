using GSS.Core.ApiBinding.Models;

namespace GSS.Core.ApiBinding.Abstractions
{
	public interface IMethodMetadata
	{
		string Name { get; }
		GssValue Invoke(object instance, ReadOnlySpan<GssValue> args);
	}
}