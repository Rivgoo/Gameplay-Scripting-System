namespace GSS.Kernel.Exceptions
{
    public sealed class MemberNotFoundException : KernelException
    {
        public MemberNotFoundException(string memberName, string context)
            : base($"Binding Error: Member '{memberName}' not found. Context: {context}") { }
    }
}