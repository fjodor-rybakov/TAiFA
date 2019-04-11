using System.Collections.Generic;

namespace Compiler.Lexer1
{
    public class AutomateData
    {
        public string Type { get; }
        public int CountStates { get; }
        public List<int> FinishStates { get; }
        public Dictionary<string, List<int>> Automate { get; }

        public AutomateData(string type, int countStates, List<int> finishStates, Dictionary<string, List<int>> automate)
        {
            Type = type;
            CountStates = countStates;
            FinishStates = finishStates;
            Automate = automate;
        }
    }
}