using System.Collections.Generic;
using Compiler.Helper.enums;

namespace Compiler.AST
{
    public class TreeNode
    {
        public TreeNode(TermType termType, ActionType actionType, string value, List<TreeNode> childNodes, int uniqueKey)
        {
            TermType = termType;
            ActionType = actionType;
            Value = value;
            ChildNodes = childNodes;
            UniqueKey = uniqueKey;
        }

        public ActionType ActionType { get; }
        public TermType TermType { get; }
        public string Value { get; }
        public List<TreeNode> ChildNodes { get; }
        public int UniqueKey { get; }
    }
}