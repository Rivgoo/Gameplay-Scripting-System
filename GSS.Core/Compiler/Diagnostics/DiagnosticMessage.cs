namespace GSS.Core.Compiler.Diagnostics
{
	public sealed class DiagnosticMessage
	{
		public DiagnosticDescriptor Descriptor { get; }
		public TextLocation Location { get; }
		public string FormattedMessage { get; }

		public DiagnosticMessage(DiagnosticDescriptor descriptor, TextLocation location, params object[] args)
		{
			Descriptor = descriptor;
			Location = location;
			FormattedMessage = string.Format(descriptor.MessageFormat, args);
		}

		public override string ToString()
			=> $"[{Descriptor.Code}] {Location}: {FormattedMessage} {Descriptor.ResolutionSuggestion}";
	}
}