using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Lexer
{
    public class Controller
    {
        private const string ERROR_MESSAGE_PARSE_COUNT_STATE = "Не удалось считать кол-во состояний";
        private const string ERROR_MESSAGE_PARSE_FINISH_STATE = "Не удалось считать финишные состояния";
        public List<AutomateData> Automates { get; }
        public List<char> BindOptions { get; } = new List<char>{'=', '!', '?', '&', '|'};
        public List<char> SplitSymbols { get; } = new List<char>{',', ':', ';', '.',  '"', '(', ')', '{', '}', '\'', '*', '/', '+', '-', '<', '>'};
        public List<string> ReserveWords { get; } = new List<string>
        {
            "begin", "end", "main", "read", "write", "writeln", "char", "bool", "integer", "for", "while", "cond", "else", "var", "program", "then"
        };

        public List<string> ComparisonWords { get; } = new List<string>
        {
            ">", "==", "!=", "<", ">=", "<="
        };

        public List<string> MathWords { get; } = new List<string>
        {
            "+", "-", "*"
        };



        public Controller(params string[] paths)
        {
            Automates = new List<AutomateData>();
            foreach (var path in paths)
            {
                var automateData = GetAutomate(path);
                Automates.Add(automateData);
            }
        }

        private AutomateData GetAutomate(string path)
        {
            var reader = new StreamReader(path);
            var type = Path.GetFileName(path).Split('.')[0];
            var countStates = int.Parse(reader.ReadLine() ?? throw new Exception(ERROR_MESSAGE_PARSE_COUNT_STATE));
            List<int> finishStates = GetFinishStates(ref reader);
            Dictionary<string, List<int>> automate = InitDictionary(ref reader);
            
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var index = 0;
                var states = line.Split(' ');
                foreach (var item in automate)
                {
                    item.Value.Add(int.Parse(states[index]));
                    index++;
                }
            }
            
            return new AutomateData(type, countStates, finishStates, automate);
        }

        private Dictionary<string, List<int>> InitDictionary(ref StreamReader reader)
        {
            var result = new Dictionary<string, List<int>>();
            var terminalSymbols = reader.ReadLine()?.Split(' ');
            if (terminalSymbols == null) return result;
            foreach (var symbol in terminalSymbols)
            {
                result.Add(symbol, new List<int>());
            }

            return result;
        }
        

        private List<int> GetFinishStates(ref StreamReader reader)
        {
            var result = new List<int>();
            var states = reader.ReadLine()?.Split(' ');
            if (states == null) return result;
            result.AddRange(states.Select(state => int.Parse(state ?? throw new Exception(ERROR_MESSAGE_PARSE_FINISH_STATE))));

            return result;
        }
    }
}