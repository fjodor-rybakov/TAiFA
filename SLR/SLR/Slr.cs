using System;
using System.Collections.Generic;
using System.Text;

namespace SLR
{
    class Slr
    {
        List<string> _elements;
        List<string> _identifiers = new List<string>();
        List<Dictionary<string, List<String>>> _rules;

        public Slr(List<Dictionary<string, List<String>>> rules)
        {
            _rules = rules;
        }

        public void SyntexAnalyze()
        {
            AddIdentifiers();

        }

        private void AddnewIdentifier(string identifier)
        {
            if (!_identifiers.Exists(x => x.Contains(identifier)))
            {
                _identifiers.Add(identifier);
            }
        }



        private void AddIdentifiers()
        {
            foreach (var rule in _rules)
            {
                var keys = rule.Keys;

                foreach (var key in keys)
                {
                    AddnewIdentifier(key);
                    List<string> elements = new List<string>();
                    rule.TryGetValue(key, out elements);
                    foreach (var element in elements)
                    {
                        AddnewIdentifier(element);
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
