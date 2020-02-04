using System.Collections.Generic;
using Compiler.Enums;
using Compiler.InsertActionsInSyntax.ASTNodes.Enums;

namespace Compiler.InsertActionsInSyntax.ASTNodes
{
	public class TreeNode : IASTNode
	{
		public NodeType NodeType { get; }

		public TermType TermType { get; }

		public string Value { get; }

		public List<IASTNode> Nodes { get; }

		public TreeNode(NodeType nodeType, TermType termType, List<IASTNode> nodes, string value = null)
		{
			NodeType = nodeType;
			TermType = termType;
			Nodes = nodes;
			Value = value;
		}
	}
}
