using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Lexer1
{
    public class Controller
    {
        private const string ErrorMessageParseCountState = "Не удалось считать кол-во состояний";
        private const string ErrorMessageParseFinishState = "Не удалось считать финишные состояния";
        public List<AutomateData> Automates { get; }
        public List<char> BindOptions { get; } = new List<char>{'=', '*', '/', '+', '-', '<', '>'};

        public Controller(params string[] paths)
        {
            Automates = new List<AutomateData>();
            foreach (var path in paths)
            {
                AutomateData automateData = GetAutomate(path);
                Automates.Add(automateData);
            }
        }

        public AutomateData GetAutomate(string path)
        {
            StreamReader reader = new StreamReader(path);
            string type = Path.GetFileName(path).Split('.')[0];
            int countStates = int.Parse(reader.ReadLine() ?? throw new Exception(ErrorMessageParseCountState));
            List<int> finishStates = GetFinishStates(ref reader);
            Dictionary<string, List<int>> automate = InitDictionary(ref reader);
            
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                int index = 0;
                string[] states = line.Split(' ');
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
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            string[] terminalSymbols = reader.ReadLine()?.Split(' ');
            if (terminalSymbols == null) return result;
            foreach (var symbol in terminalSymbols)
            {
                result.Add(symbol, new List<int>());
            }

            return result;
        }
        

        private List<int> GetFinishStates(ref StreamReader reader)
        {
            List<int> result = new List<int>();
            string[] states = reader.ReadLine()?.Split(' ');
            if (states == null) return result;
            result.AddRange(states.Select(state => int.Parse(state ?? throw new Exception(ErrorMessageParseFinishState))));

            return result;
        }
    }
}