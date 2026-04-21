namespace GSS.Kernel.Api.Abstractions
{
    public sealed class VoidResult
    {
        public static readonly VoidResult Instance = new();
        private VoidResult() { }
    }
}