using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler.LLSyntaxer
{
    public class Generator
    {
        private const string PATH_RULES = @"../../../files/generator/rules.txt";
        private const string FINISH_STATE = "[end]";
        private const string EMPTY_SYMBOL = "E";
        private readonly List<NodeInfo> _table = new List<NodeInfo>();
        private readonly List<string> _list = new List<string>();
        private readonly Dictionary<string, List<string>> _dirSet = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<int>> _positions = new Dictionary<string, List<int>>();

        public Generator()
        {
            ReadRules();
            var c = 0;
            foreach (var n in _table)
            {
                Console.Write("Index: " + c + " ");
                c++;
                Console.Write("Name: " + n.Name + " ");
                Console.Write("DirSet: ");
                foreach (var s in n.DirSet)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine("IsShift: " + n.IsShift + " ErrGo: " + n.IfErrGoTo  + " STack: " + n.IsStack + " goto: " + n.GoTo +  " end: " + n.IsEnd  );
            }

            var runner = new Runner(_table);
            var res = runner.Run();
            Console.WriteLine("result: " + res);
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

            while ((line = reader.ReadLine()) != null)
            {
                var isLeftPart = true;
                var isDirSet = false;
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
                        IsEnd = lexem == FINISH_STATE,
                        IsShift = IsTerminal(lexem) && lexem != FINISH_STATE && lexem != EMPTY_SYMBOL,
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
            if (lexem != EMPTY_SYMBOL) return !IsTerminal(lexem) ? _dirSet[formatted] : new List<string> {lexem};
            var result = new List<string>();
            var count = index + 2;
            while (_list[count] != "^")
            {
                result.Add(_list[count]);
                count++;
            }

            return result;

        }
        private int GetGoTo(string lexem, string formatted, int index)
        {
            if (lexem == FINISH_STATE || IsTerminal(lexem) && IsLast(lexem, index) || lexem == EMPTY_SYMBOL)
            {
                return -1;
            }

            return IsTerminal(lexem) ? _table.Count + 1 : _positions[formatted][0];
        }

        private int GetErrGo(string leftPart, string lexem)
        {
            var errGo = -1;
            if (leftPart == "" || lexem == "->") return errGo;
            var pos = leftPart.Replace("<", "").Replace(">", "");
            var index = _positions[pos].IndexOf(_table.Count);
            if (index != -1 && _positions[pos].Count - 1 >= index + 1)
            {
                errGo = _positions[pos][index + 1];
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