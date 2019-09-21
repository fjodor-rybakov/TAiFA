using System;
using System.Collections.Generic;
using System.Text;

namespace SLR
{
    class Slr
    {
        List<string> _elements;
        List<Dictionary<string, List<String>>> _rules;

        public Slr(List<Dictionary<string, List<String>>> rules)
        {
            _rules = rules;
        }

        public void analyze()
        {
            showRules();
        }

        private void showRules()
        {
            foreach (var rule in _rules)
            {
                var keys = rule.Keys;

                foreach (var key in keys)
                {
                    Console.WriteLine(key + ":");
                    List<string> elements = new List<string>();
                    rule.TryGetValue(key, out elements);
                    foreach (var element in elements)
                    {
                        Console.WriteLine(element);
                    }
                }
            }
        }

        private bool isTerminal(string value)
        {
            return value[0] != '<' && value[value.Length - 1] != '>';
        }
    }
}
