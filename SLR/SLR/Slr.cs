using System;
using System.Collections.Generic;
using System.Text;

namespace SLR
{
    class Slr
    {
        Dictionary<string, List<string>> _valueOfDict;
        List<string> _elements;
        List<List<string>> _addingIdentifiersHistory = new List<List<string>>();
        List<string> _identifiers = new List<string>();
        List<Dictionary<string, List<String>>> _rules;
        List<Dictionary<List<String>, Dictionary<string, List<String>>>> _resultTable = new List<Dictionary<List<String>, Dictionary<string, List<string>>>>();
        Dictionary<string, List<string>> dictForInsertToDict = new Dictionary<string, List<string>>();
        Dictionary<List<string>, Dictionary<string, List<string>>> dictForInsertResultTable = new Dictionary<List<string>, Dictionary<string, List<string>>>();


        public Slr(List<Dictionary<string, List<String>>> rules)
        {
            _rules = rules;
        }

        public void SyntexAnalyze()
        {
            AddIdentifiers();
            AddTestItem();
            ShowResultTable();
        }
        
        private void AddTestItem()
        {
            dictForInsertToDict.Add("id", new List<string>(new string[] { "id11", "id12" }));
            dictForInsertToDict.Add("<Z>", new List<string>(new string[] { "<Z>1", "<Z>2" }));

            dictForInsertResultTable.Add(new List<string>(new string[] { "id11" }), dictForInsertToDict);
            _resultTable.Add(dictForInsertResultTable);
        }

        private void ShowResultTable()
        {
            foreach(var row in _resultTable)
            {
                var keys = row.Keys;
                
                foreach(var key in keys)
                {
                    row.TryGetValue(key, out _valueOfDict);

                    var stringKey = "";
                    key.ForEach(x => { stringKey = stringKey + " " + x; });
                    Console.WriteLine(stringKey + ":");

                    var keysOnDict = _valueOfDict.Keys;

                    ShowValueOfDict(keysOnDict);
                }
            }
        }

        private void ShowValueOfDict(Dictionary<string, List<String>>.KeyCollection keys)
        {
            foreach(var key in keys)
            {
                Console.Write("     " + key + ":");
                _valueOfDict.TryGetValue(key, out _elements);
                
                _elements.ForEach(element =>
                {
                    Console.Write(element + " ");
                });

                Console.WriteLine();
            }
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
