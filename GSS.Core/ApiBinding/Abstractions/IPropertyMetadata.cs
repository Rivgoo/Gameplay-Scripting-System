using GSS.Core.ApiBinding.Models;

namespace GSS.Core.ApiBinding.Abstractions
{
	public interface IPropertyMetadata
	{
		string Name { get; }
		bool CanRead { get; }
		bool CanWrite { get; }
		GssType Type { get; }
		GssValue GetValue(object instance);
		void SetValue(object instance, GssValue value);
	}
}