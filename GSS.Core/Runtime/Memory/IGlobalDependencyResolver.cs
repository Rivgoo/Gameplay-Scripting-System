namespace GSS.Core.Runtime.Memory
{
	public interface IGlobalDependencyResolver
	{
		T Resolve<T>();
	}
}