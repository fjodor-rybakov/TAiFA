using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compiler.SLR
{
    struct Response
    {
        public string key;
        public string value;
        public bool isLast;
    }

    struct Value
    {
        public string columnOfTable;
        public List<string> valueOfColumn;
    }
    struct Table
    {
        public List<string> key;
        public List<Value> value;

    }

    struct ReturnData
    {
        public List<Dictionary<string, List<string>>> rules;
        public List<Table> resultTable;
    }

    class Slr
    {
        List<string> _identifiers = new List<string>();
        List<Dictionary<string, List<string>>> _rules;
        List<Table> _resultTable = new List<Table>();
        List<Value> columnsOnTable = new List<Value>();

        public Slr(List<Dictionary<string, List<String>>> rules)
        {
            _rules = rules;
        }

        public ReturnData GetTable()
        {
            AddIdentifiers();
            FillResultTable();
            ShowResultTable(); //раскомментировать, если хочешь увидеть таблицу.
            ReturnData returnData = new ReturnData();
            returnData.resultTable = _resultTable;
            returnData.rules = _rules;
            return returnData;
        }

        private void FillResultTable()
        {
            DoFirstIteration();
            DoOtherIterations();
        }

        private bool isValid(List<Table> table, List<string> list)
        {
            foreach (var row in table)
            {
                if (row.key.SequenceEqual(list))
                {
                    return false;
                }
            }

            return true;
        }

        private void ShowKeys()
        {
            _resultTable.ForEach(x => { Console.WriteLine(x.key[0]); });
        }

        private void DoOtherIterations()
        {
            int i = 0;
            while (true)
            {
                var rtTemp = new List<Table>(_resultTable);
                if (i > rtTemp.Count - 1)
                {
                    break;
                }

                rtTemp[i].value.ForEach(row =>
                {
                    if (row.valueOfColumn.Count != 0)
                    {
                        if (!row.valueOfColumn[0].Contains("RETURN") && !row.valueOfColumn[0].Contains("OK"))
                        {
                            if (isValid(_resultTable, row.valueOfColumn))
                            {
                                Table table = new Table();
                                table.key = row.valueOfColumn;
                                table.value = GetEmptyColumns();
                                _resultTable.Add(table);
                            }
                        }
                    }
                });

                if (i + 1 < _resultTable.Count)
                {
                    var values = GetEmptyColumns();

                    _resultTable[i + 1].key.ForEach(key => {
                        int k = -1;
                        int l = -1;
                        int.TryParse(key.Split('^')[1], out k);
                        int.TryParse(key.Split('^')[2], out l);
                        var elem = GetElementOfRule(k, l);
                        //Console.WriteLine(elem.key);
                        if (elem.isLast)
                        {
                            var responseList = ToAddReturnToColumns(elem);

                            responseList.ForEach(x =>
                            {
                                AddValueToColumn(ref values, x.value, "RETURN");
                            });

                            AddValueToColumn(ref values, "__end", "RETURN");
                        }
                        else
                        {
                            elem = GetElementOfRule(k, l + 1);
                            ToProcessElementFirstIt(k, l + 1, elem, ref values);
                        }

                    });

                    _resultTable[i + 1].value.Clear();
                    _resultTable[i + 1].value.AddRange(values);

                }

                i++;
            }
        }

        private List<Response> ToAddReturnToColumns(Response elem)
        {
            Queue<Response> keysToConvolute = new Queue<Response>();
            keysToConvolute.Enqueue(elem);
            List<Response> responseList = new List<Response>();
            List<List<string>> history = new List<List<string>>();//для проверки на повторения;[0] - строки,которые сворачиваем a->c (a), возвращаемый лист;
            history.Add(new List<string>());
            history.Add(new List<string>());
            history[0].Add(elem.key);


            while (keysToConvolute.Count != 0)
            {
                Response key = keysToConvolute.Dequeue();
                responseList.AddRange(GetColumnsToConvolute(ref keysToConvolute, key, history));
            }

            return responseList;
        }

        private List<Response> GetColumnsToConvolute(ref Queue<Response> keysToConvolute, Response key, List<List<string>> history)
        {
            List<Response> responses = new List<Response>();

            for (int i = 0; i < _rules.Count; i++)
            {
                int j = 0;
                Response elem = GetElementOfRule(i, j);

                while (elem.value != "-1")
                {
                    if (elem.value == key.key && !elem.isLast)
                    {
                        if (!history[1].Contains(GetElementOfRule(i, j + 1).value))
                        {
                            responses.Add(GetElementOfRule(i, j + 1));
                        }
                    }

                    if (elem.value == key.key && elem.isLast)
                    {
                        if (!history[0].Contains(elem.key))
                        {
                            keysToConvolute.Enqueue(elem);
                        }
                    }

                    j++;
                    elem = GetElementOfRule(i, j);
                }
            }

            return responses;
        }

        private void DoFirstIteration()
        {
            string key = "";
            List<Value> values = new List<Value>();
            List<Value> mainValues = new List<Value>();
            values = GetEmptyColumns();

            for (int i = 0; i < _rules.Count; i++)
            {
                var elem = GetElementOfRule(i, 0);
                key = elem.key;

                ToProcessElementFirstIt(i, 0, elem, ref values);


                mainValues.ForEach(main =>
                {
                    values.ForEach(val =>
                    {
                        if (main.columnOfTable == val.columnOfTable)
                        {
                            main.valueOfColumn.AddRange(val.valueOfColumn);
                        }
                    });
                });

                if (GetElementOfRule(i + 1, 0).key != elem.key || GetElementOfRule(i + 1, 0).value == "-1")
                {
                    break;
                }
            }

            AddValueToColumn(ref values, key, "OK");
            Table row = new Table();
            row.key = new List<string>(new string[] { key });
            row.value = values;
            _resultTable.Add(row);

        }

        private bool AddValueToColumn(ref List<Value> values, string key, string value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].columnOfTable == key)
                {
                    if (!values[i].valueOfColumn.Contains(value))
                    {
                        values[i].valueOfColumn.Add(value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }


        private void ToProcessElementFirstIt(int i, int j, Response elem, ref List<Value> columns)
        {

            if (IsTerminal(elem.value))
            {
                AddValueToColumn(ref columns, elem.value, elem.value + "^" + i + "^" + j);
            }
            else
            {
                AddValueToColumn(ref columns, elem.value, elem.value + "^" + i + "^" + j);
                var notTerminals = GetAllNotTerminalId(elem.value, new List<string>(), 0);

                if (!notTerminals.Contains(elem.value))
                {
                    notTerminals.Add(elem.value);
                }

                for (var f = 0; f < notTerminals.Count; f++)
                {
                    for (var s = 0; s < _rules.Count; s++)
                    {
                        var elemR = GetElementOfRule(s, 0);

                        if (elemR.key == notTerminals[f])
                        {
                            AddValueToColumn(ref columns, elemR.value, elemR.value + "^" + s + "^" + 0);
                        }
                    }
                }
            }
        }

        private List<string> GetAllNotTerminalId(string notTerminal, List<string> listOfNeterminals, int count)
        {
            List<string> notTerminals = new List<string>(listOfNeterminals);
            List<string> tempNotTerminals = new List<string>(listOfNeterminals);

            for (int i = 0; i < _rules.Count; i++)
            {
                var elem = GetElementOfRule(i, 0);
                if (elem.key == notTerminal)
                {
                    if (!IsTerminal(elem.value) && !tempNotTerminals.Contains(elem.value))
                    {
                        tempNotTerminals.Add(elem.value);
                        notTerminals.Add(elem.value);
                    }
                }
            }

            notTerminals.ForEach(x => {
                for (int i = 0; i < _rules.Count; i++)
                {
                    var elem = GetElementOfRule(i, 0);
                    if (elem.key == x)
                    {
                        if (!IsTerminal(elem.value) && !tempNotTerminals.Contains(elem.value))
                        {
                            tempNotTerminals.Add(elem.value);
                        }
                    }
                }
            });

            if (count == tempNotTerminals.Count)
            {
                return tempNotTerminals;
            }

            listOfNeterminals.Clear();
            listOfNeterminals.AddRange(GetAllNotTerminalId("", new List<string>(tempNotTerminals), tempNotTerminals.Count));

            return tempNotTerminals;
        }


        private Response GetElementOfRule(int i, int j)
        {
            List<string> elements = new List<string>();

            Response response = new Response();
            response.value = "-1";

            if (i >= _rules.Count)
            {
                return new Response();
            }

            var rule = _rules[i];
            var keys = rule.Keys;

            foreach (var key in keys)
            {
                response.key = key;
                rule.TryGetValue(key, out elements);
            }

            if (j >= elements.Count)
            {
                return response;
            }

            response.value = elements[j];
            if (j == elements.Count - 1)
            {
                response.isLast = true;
            }
            else
            {
                response.isLast = false;
            }

            return response;
        }

        private List<Value> GetEmptyColumns()
        {
            List<Value> values = new List<Value>();

            foreach (var id in _identifiers)
            {
                Value value = new Value();
                value.columnOfTable = id;
                value.valueOfColumn = new List<string>();
                values.Add(value);
            }

            return values;
        }

        private void ShowResultTable()
        {
            //var writer = new StreamWriter("test.txt");
           
            _resultTable.ForEach(row => {
                string stringKey = "";
                row.key.ForEach(x => { stringKey = "( " + stringKey + x + " ) "; });
                Console.WriteLine(stringKey + ":");

                row.value.ForEach(x =>
                {
                    Console.Write("     " + x.columnOfTable + "^ ");
                    x.valueOfColumn.ForEach(y => { Console.Write(y + " "); });
                    Console.WriteLine();
                });

            });

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
            _identifiers.Add("__end");
        }

        private bool IsTerminal(string value)
        {
            return value[0] != '<' && value[value.Length - 1] != '>';
        }
    }
}
