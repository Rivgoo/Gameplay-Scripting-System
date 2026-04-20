using GSS.Core.Exceptions;

namespace GSS.Core.Compiler.Exceptions
{
	public sealed class GssCompilationCanceledException : GssException
	{
		public GssCompilationCanceledException(string reason)
			: base($"Compilation was canceled: {reason}") { }
	}
}