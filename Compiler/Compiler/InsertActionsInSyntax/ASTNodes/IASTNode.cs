using System.Collections.Generic;
using Compiler.Enums;
using Compiler.InsertActionsInSyntax.ASTNodes.Enums;

namespace Compiler.InsertActionsInSyntax.ASTNodes
{
    public interface IASTNode
    {
        NodeType NodeType { get; }
        TermType TermType { get; }
        string Value { get; }
        List<IASTNode> Nodes { get; }
    }
}
