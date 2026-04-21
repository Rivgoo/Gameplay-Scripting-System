namespace GSS.Runtime.Engine
{
    public interface IGlobalResolver
    {
        T Resolve<T>();
    }
}