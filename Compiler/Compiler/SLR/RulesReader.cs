using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

struct HeadOfRule
{
    public string haedOfRule;
    public string action;
}

namespace Compiler.SLR
{
    class RulesReader
    {
        private const string PATH_RULES = @"../../../files/dirSetRules.txt";
        private List<Dictionary<HeadOfRule, List<string>>> _rules = new List<Dictionary<HeadOfRule, List<String>>>();

        public List<Dictionary<HeadOfRule, List<string>>> GetRules()
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
                HeadOfRule leftPart = new HeadOfRule();
                string[] partsOne;

                if(line.IndexOf("$") != -1)
                {
                    var parts = line.Replace("->", "~").Split('$');
                    leftPart.action = parts[1];
                    partsOne = parts[0].Split('~');
                }
                else
                {
                    partsOne = line.Replace("->", "~").Split('~');
                }

               
                if (partsOne.Length != 2)
                {
                    return;
                }

                leftPart.haedOfRule = partsOne[0].Split(' ')[0];
                
                var words = partsOne[1].Split(' ');
                List<string> wordsList = new List<string>();

                foreach (var word in words)
                {
                    wordsList.Add(word);
                }

                Dictionary<HeadOfRule, List<string>> newDict = new Dictionary<HeadOfRule, List<string>>();
                newDict.Add(leftPart, wordsList);

                _rules.Add(newDict);
            }
        }
    }
}
