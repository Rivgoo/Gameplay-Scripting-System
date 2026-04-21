namespace GSS.Kernel.Diagnostics
{
    public sealed class DiagnosticDescriptor
    {
        public DiagnosticCode Code { get; }
        public DiagnosticSeverity Severity { get; }
        public string MessageFormat { get; }
        public string ResolutionSuggestion { get; }

        public DiagnosticDescriptor(DiagnosticCode code, DiagnosticSeverity severity, string messageFormat, string resolutionSuggestion)
        {
            Code = code;
            Severity = severity;
            MessageFormat = messageFormat;
            ResolutionSuggestion = resolutionSuggestion;
        }
    }
}