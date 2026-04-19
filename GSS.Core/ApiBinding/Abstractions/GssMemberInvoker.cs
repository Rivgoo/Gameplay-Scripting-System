using GSS.Core.ApiBinding.Models;

namespace GSS.Core.ApiBinding.Abstractions
{
	public delegate GssValue GssMemberInvoker(object instance, ReadOnlySpan<GssValue> args);
}