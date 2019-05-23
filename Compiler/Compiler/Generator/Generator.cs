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
        private readonly List<Tuple<string, string[]>> _list = new List<Tuple<string, string[]>>();

        public Generator()
        {
            ReadRules();
        }

        private void ReadRules()
        {
            var reader = new StreamReader(PATH_RULES);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var lexems = line.Split(' ');
                _list.Add(new Tuple<string, string[]>(lexems[0], lexems.Skip(2).ToArray()));
            }

            // Console.WriteLine("This terminal: " + IsTerminal(item.Item2[0]) + ", Value: " + item.Item2[0]);
            // var item = _list[0];

            foreach (var key in _list)
            {
                _table.Add(new NodeInfo
                {
                    Number = 0,
                    Name = key.Item1,
                    GuidesSet = new List<string>{key.Item2[0]},
                    IsError = false,
                    IsFinish = false,
                    IsShift = true,
                    Children = GetChildren(key.Item2)
                });

                foreach (var item in key.Item2)
                {
                    _table.Add(new NodeInfo
                    {
                        Number = 0,
                        Name = key.Item1,
                        GuidesSet = new List<string>{key.Item2[0]},
                        IsError = false,
                        IsFinish = false,
                        IsShift = true,
                        Children = GetChildren(key.Item2)
                    });
                }
            }
        }

        private List<NodeInfo> GetChildren(IEnumerable<string> children)
        {
            foreach (var child in children)
            {
                if (IsTerminal(child))
                {
                    var nodeInfo = new NodeInfo
                    {
                        Number = 1,
                        Name = child,
                        GuidesSet = new List<string>{child},
                        IsError = false,
                        IsFinish = false,
                        IsShift = true,
                        Children = GetChildren(lastNode, null)
                    };
                }
                else
                {
                    var nodeInfo = new NodeInfo
                    {
                        Number = 1,
                        Name = child,
                        GuidesSet = new List<string>{child},
                        IsError = false,
                        IsFinish = false,
                        IsShift = true,
                        Children = GetChildren(null)
                    };
                }
            }
        }

        private static bool IsTerminal(string value)
        {
            return value[0] != '<' && value[value.Length - 1] != '>';
        }
    }
}