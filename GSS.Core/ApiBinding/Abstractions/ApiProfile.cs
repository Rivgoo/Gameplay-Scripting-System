using GSS.Core.ApiBinding.Builders;

namespace GSS.Core.ApiBinding.Abstractions
{
	public abstract class ApiProfile<T>
	{
		public abstract void Build(IApiBindingScope<T> scope);
	}
}