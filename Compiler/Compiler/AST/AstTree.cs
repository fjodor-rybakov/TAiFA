using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Helper.enums;
using Compiler.SLR;

namespace Compiler.AST
{
    public class AstTree
    {
        private readonly Stack<TreeNode> _tree = new Stack<TreeNode>();

        public void CreateNode(ColumnOfReestr registry, string value, int countRemoveElems)
        {
            switch (registry.nameOfFunction)
            {
                case nameof(ActionType.DefineLeaf):
                {
                    _tree.Push(new TreeNode(TermType.Int, ActionType.DefineLeaf, value, null));
                    break;
                }
                case nameof(ActionType.DefineUnaryMinus):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Minis, ActionType.DefineUnaryMinus, "-", tempList));
                    break;
                }
                case nameof(ActionType.Sum):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Plus, ActionType.Sum, "+", tempList));
                    break;
                }
                case nameof(ActionType.Subtract):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Minis, ActionType.Subtract, "-", tempList));
                    break;
                }
                case nameof(ActionType.Multiplication):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Multiple, ActionType.Multiplication, "*", tempList));
                    break;
                }
                case nameof(ActionType.Division):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Division, ActionType.Division, "/", tempList));
                    break;
                }
                case nameof(ActionType.DefineBody):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Multiple, ActionType.DefineBody, value, tempList));
                    break;
                }
                case nameof(ActionType.DefineVar):
                {
                    var tempList = new List<TreeNode>();
                    if (tempList.Count != 0)
                        for (var i = 0; i < countRemoveElems; i++) tempList.Add(_tree.Pop());
                    _tree.Push(new TreeNode(TermType.Multiple, ActionType.DefineVar, value, tempList));
                    break;
                }
                case null:
                {
                    break;
                }
                default:
                    throw new Exception("Unknow action! Please add action at enum \"ActionType\" and release action at AST tree");
            }
        }

        public void PrintTree()
        {
            Helper.Helper.PrintDelimiter(54);
            Console.WriteLine($"|{"Node value", -10}|{"Children list", -20}|{"Action", -20}|");
            Helper.Helper.PrintDelimiter(54);
            Traversal(_tree.First());
            Helper.Helper.PrintDelimiter(54);
        }

        private void Traversal(TreeNode node)
        {
            if (node == null) return;
            if (node.ChildNodes != null)
                foreach (var elem in node.ChildNodes)
                {
                    Traversal(elem);
                }
            var childrenListString = node.ChildNodes == null ? "null" : string.Join(", ", node.ChildNodes.Select(item => item.Value));
            Console.WriteLine($"|{node.Value, -10}|{childrenListString, -20}|{node.ActionType, -20}|");
        }

        public TreeNode GetTree()
        {
            return _tree.Peek();
        }
    }
}