using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Generator
{
    public class DirSetFinder
    {
        private const string PATH_RULES = @"../../../files/generator/dirSetRules.txt";
        private readonly Dictionary<string, List<string>> _rules = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<List<string>>> _fullRules = new Dictionary<string, List<List<string>>>();
        private Dictionary<string, List<string>> _result = new Dictionary<string, List<string>>();
        private string _curr = "";
        public void Find()
        {
            ReadRules();
            FindDirSets();
            var tmp = _result;
            _result = new Dictionary<string, List<string>>();
            foreach (var elem in tmp)
            {
                _result.Add(elem.Key, new List<string>(elem.Value.Distinct()));
            }

            foreach (var VARIABLE in _result)
            {
                Console.WriteLine(VARIABLE.Key);
                foreach (var v in VARIABLE.Value)
                {
                   Console.Write(v + " ");
                }
                Console.WriteLine();
            }
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

                var leftPart = parts[0].Split(' ')[0];
                var first = parts[1].Split(' ')[0];
                var right = parts[1].Split(' ');
                if (!_rules.ContainsKey(leftPart))
                {
                    _rules.Add(leftPart, new List<string>());
                    _fullRules.Add(leftPart, new List<List<string>>());
                }
                _rules[leftPart].Add(first);
                _fullRules[leftPart].Add(new List<string>(right));
            }
        }

        private void FindDirSets()
        {
            List<string> res, set;
            foreach (var term in _rules)
            {
                res = new List<string>();
                set = new List<string>();
                _curr = term.Key;
                if (!_result.ContainsKey(term.Key))
                {
                    _result.Add(term.Key, new List<string>());
                }
                try
                {
                    Console.WriteLine(term.Key);
                    FindFirst(term.Value, res, set, term.Key);
                    _result[term.Key].AddRange(res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }
        }

        private void FindFirst(List<string> values, List<string> result, List<string> set, string term)
        {
            foreach (var value in values)
            {
                if (value == "E")
                {
                    FindFollow(term, result);
                }
                else if (isTerminal(value))
                {
                    result.Add(value);
                }
                else
                {
                    if (set.Contains(value))
                    {
                        throw new Exception("Cycle");
                    }
                    set.Add(value);
                    Console.WriteLine(value);
                    FindFirst(_rules[value], result, set, value);
                }
            }
        }

        private void FindFollow(string term, List<string> result)
        {
            if (!_result.ContainsKey(term))
            {
                _result.Add(term, new List<string>());
            }
            foreach (var values in _fullRules)
            {
                foreach (var right in values.Value)
                {
                    var count = 0;
                    //var res = new List<string>();
                    foreach (var value in right)
                    {
                        if (value == term)
                        {
                            if (count == right.Count - 1)
                            {
//                                Console.WriteLine($"{value}, {term}");
                                if (values.Key != term)
                                {
                                    FindFollow(values.Key, result);
                                }
                                //_result[_curr].AddRange(res);
                            }
                            else
                            {
                                var next = right[count + 1];
                                if (isTerminal(next))
                                {
                                    result.Add(next);
                                }
                                else
                                {
                                    FindFirst(_rules[next], result, new List<string>(), next);
                                    result.AddRange(result);
                                }
                            }
                        }
                        count++;
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