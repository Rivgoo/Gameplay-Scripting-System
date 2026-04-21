namespace GSS.Kernel.Compiler.Syntax
{
    public interface IAstNode
    {
        int Kind { get; }
        IEnumerable<IAstNode> GetChildren();
    }
}