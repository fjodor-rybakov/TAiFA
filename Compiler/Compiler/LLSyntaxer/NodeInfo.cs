using System.Collections.Generic;

namespace Compiler.LLSyntaxer
{
    public class NodeInfo
    {
        public List<string> DirSet { get; set; }
        public string Name { get; set; }
        public bool IsShift { get; set; }
        public bool IsEnd { get; set; }
        public bool IsStack { get; set; }
        public int GoTo { get; set; }
        public int IfErrGoTo { get; set; }
    }
}