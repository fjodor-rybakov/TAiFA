using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Generator
{
    public class Generator
    {
        private const string PATH_RULES = @"../../../files/generator/rules.txt";
        private readonly List<NodeInfo> _table = new List<NodeInfo>();
        private readonly List<string> _list = new List<string>();
        private readonly Dictionary<string, List<string>> _dirSet = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<int>> _positions = new Dictionary<string, List<int>>();

        public Generator()
        {
            ReadRules();
            foreach (var n in _table)
            {
                Console.Write(n.Name + " ");
                foreach (var s in n.DirSet)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine( n.IsShift + " " + n.IfErrGoTo  + " " + n.IsStack + " " + n.GoTo +  " " + n.IsEnd  );
            }
        }

        private void ReadRules()
        {
            FillDirSetsAndPositions();
            FillTable();
        }

        private void FillDirSetsAndPositions()
        {
            var reader = new StreamReader(PATH_RULES);
            string line, value = "";
            var counter = 0;
            bool isLeftPart, isDirSet;

            while ((line = reader.ReadLine()) != null)
            {
                isLeftPart = true;
                isDirSet = false;
                var lexems = line.Split(' ');
                foreach (var lexem in lexems)
                {
                    _list.Add(lexem);
                    if (isDirSet && value.Length != 0 && !_dirSet[value].Contains(lexem))
                    {
                        _dirSet[value].Add(lexem);
                    }

                    if (isLeftPart && lexem != "->")
                    {
                        value = lexem.Replace("<", "").Replace(">", "");
                        AddTerminal(value, counter);
                    }

                    if (!isLeftPart && !isDirSet && lexem != "/")
                    {
                        counter++;
                    }

                    switch (lexem)
                    {
                        case "->":
                            isLeftPart = false;
                            break;
                        case "/":
                            isDirSet = true;
                            break;
                        default: break;
                    }
                }
                _list.Add("^"); // mark end of line
            }
        }

        private void FillTable()
        {
            bool isLeftPart = true, isDirSet = false;
            var leftPart = "";
            var index = -1;
            
            foreach (var lexem in _list)
            {
                index++;
                if (isLeftPart && lexem != "->")
                {
                    leftPart = lexem;
                }
                if (lexem == "^")
                {
                    isLeftPart = true;
                    isDirSet = false;
                }
                
                if (lexem != "^" && lexem != "=>" && lexem != "/" && !isLeftPart && !isDirSet)
                {
                    var formatted = lexem.Replace("<", "").Replace(">", "");
                    _table.Add(new NodeInfo
                    {
                        Name = formatted,
                        DirSet = GetDirSet(lexem, formatted, index),
                        GoTo = GetGoTo(lexem, formatted, index),
                        IfErrGoTo = GetErrGo(leftPart, lexem),
                        IsEnd = lexem == "[e]",
                        IsShift = IsTerminal(lexem) && lexem != "[e]" && lexem != "E",
                        IsStack = !IsTerminal(lexem) && !IsLast(lexem, index)
                    });
                }
                switch (lexem)
                {
                    case "->":
                        isLeftPart = false;
                        break;
                    case "/":
                        isDirSet = true;
                        break;
                    default: break;
                }
            }
        }

        private List<string> GetDirSet(string lexem, string formatted, int index)
        {
            if (lexem == "E")
            {
                var result = new List<string>();
                var count = index + 2;
                while (_list[count] != "^")
                {
                    result.Add(_list[count]);
                    count++;
                }

                return result;
            }

            return !IsTerminal(lexem) ? _dirSet[formatted] : new List<string> {lexem};
        }
        private int GetGoTo(string lexem, string formatted, int index)
        {
            if (lexem == "[e]" || IsTerminal(lexem) && IsLast(lexem, index) || lexem == "E")
            {
                return -1;
            }

            return IsTerminal(lexem) ? _table.Count + 1 : _positions[formatted][0];
        }

        private int GetErrGo(string leftPart, string lexem)
        {
            var errGo = -1;
            if (leftPart != "" && lexem != "->")
            {
                var pos = leftPart.Replace("<", "").Replace(">", "");
                var index = _positions[pos].IndexOf(_table.Count);
                if (index != -1 && _positions[pos].Count - 1 >= index + 1)
                {
                    errGo = _positions[pos][index + 1];
                }
            }

            return errGo;
        }

        private bool IsLast(string value, int index)
        {
            return index + 1 < _list.Count && _list[index + 1] == "/";
        }
        
        private void AddTerminal(string value, int position)
        {
            if (!_dirSet.ContainsKey(value))
            {
                _dirSet.Add(value, new List<string>());
            }

            if (!_positions.ContainsKey(value))
            {
                _positions.Add(value, new List<int>());
            }
            _positions[value].Add(position);
        }

        private static bool IsTerminal(string value)
        {
            return value[0] != '<' && value[value.Length - 1] != '>';
        }
    }
}