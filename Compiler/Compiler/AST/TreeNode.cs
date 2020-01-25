using System.Collections.Generic;
using Compiler.AST.helper.enums;

namespace Compiler.AST
{
    public class TreeNode
    {
        ActionType ActionType { get; }
        string Value { get; }
        List<TreeNode> ChildNodes { get; }
    }
}