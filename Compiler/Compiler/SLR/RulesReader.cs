using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.SLR
{
    class RulesReader
    {
        private const string PATH_RULES = @"../../../files/dirSetRules.txt";
        private List<Dictionary<string, List<string>>> _rules = new List<Dictionary<string, List<String>>>();

        public List<Dictionary<string, List<string>>> GetRules()
        {
            ReadRules();

            return _rules;
        }

        private void ReadRules()
        {
            var reader = new StreamReader(PATH_RULES);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Replace("->", "~").Split('~');
                if (parts.Length != 2)
                {
                    return;
                }

                string leftPart = parts[0].Split(' ')[0];
                var words = parts[1].Split(' ');
                List<string> wordsList = new List<string>();

                foreach (var word in words)
                {
                    wordsList.Add(word);
                }

                Dictionary<string, List<string>> newDict = new Dictionary<string, List<string>>();
                newDict.Add(leftPart, wordsList);

                _rules.Add(newDict);
            }
        }
    }
}
