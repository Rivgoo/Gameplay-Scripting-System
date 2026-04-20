namespace GSS.Core.Compiler.Diagnostics
{
	public sealed class DiagnosticBag
	{
		private readonly List<DiagnosticMessage> _diagnostics = new();

		public IReadOnlyList<DiagnosticMessage> Diagnostics => _diagnostics;
		public bool HasErrors => _diagnostics.Exists(d => d.Descriptor.Severity == DiagnosticSeverity.Error);

		public void Report(DiagnosticDescriptor descriptor, TextLocation location, params object[] args)
		{
			_diagnostics.Add(new DiagnosticMessage(descriptor, location, args));
		}

		public void AddRange(DiagnosticBag other)
		{
			_diagnostics.AddRange(other.Diagnostics);
		}

		public void Clear() => _diagnostics.Clear();
	}
}