using System.Collections.Generic;
using Compiler.Helper.enums;

namespace Compiler.AST
{
    public class TreeNode
    {
        public TreeNode(ActionType actionType, string value, List<TreeNode> childNodes)
        {
            ActionType = actionType;
            Value = value;
            ChildNodes = childNodes;
        }

        public ActionType ActionType { get; }
        public string Value { get; }
        public List<TreeNode> ChildNodes { get; }
    }
}