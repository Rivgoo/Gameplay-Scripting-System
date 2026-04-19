using GSS.Core.ApiBinding.Models;

namespace GSS.Core.ApiBinding.Abstractions
{
	public interface ITypePacker<T>
	{
		GssValue Pack(T value);
		T Unpack(GssValue value);
	}
}