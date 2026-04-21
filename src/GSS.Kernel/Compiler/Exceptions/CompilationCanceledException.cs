using GSS.Kernel.Exceptions;

namespace GSS.Kernel.Compiler.Exceptions
{
    public sealed class CompilationCanceledException : KernelException
    {
        public CompilationCanceledException(string reason)
            : base($"Compilation was canceled: {reason}") { }
    }
}