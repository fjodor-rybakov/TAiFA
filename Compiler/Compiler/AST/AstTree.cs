using System.Collections.Generic;

namespace Compiler.AST
{
    public class AstTree
    {
        private readonly Stack<TreeNode> _tree = new Stack<TreeNode>();

        public void PushTreeStack(TreeNode value)
        {
            _tree.Push(value);
        }
    }
}