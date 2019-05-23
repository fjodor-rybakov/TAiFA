using System.Collections.Generic;

namespace Compiler.Generator
{
    public class NodeInfo
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public List<string> GuidesSet { get; set; }
        public bool IsShift { get; set; }
        public bool IsError { get; set; }
        public bool IsFinish { get; set; }
        public List<NodeInfo> Children { get; set; }
    }
}