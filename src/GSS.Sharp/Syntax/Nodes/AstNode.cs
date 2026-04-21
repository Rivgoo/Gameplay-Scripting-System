using GSS.Kernel.Compiler.Syntax;
using GSS.Kernel.Diagnostics;

namespace GSS.Sharp.Syntax.Nodes
{
    public abstract class AstNode : IAstNode
    {
        public abstract SyntaxKind Kind { get; }
        int IAstNode.Kind => (int)Kind;
        public abstract TextSpan Span { get; }
        public abstract IEnumerable<IAstNode> GetChildren();
    }
}