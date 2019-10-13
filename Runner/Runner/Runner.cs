using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Runner
{
    struct Value
    {
        public string columnOfTable; // элемент сверху
        public List<string> valueOfColumn; // значения этого элменета
    }
    struct Table
    {
        public List<string> key; // единственный ключ, в котором может быть несколько элементов  //key:номерПравила:колонка (a:3:0)
        public List<Value> value; // сама таблица

    }

    class Runner
    {
        private string strSeparator = ",";
        private Stack enterChain = new Stack();
        private Stack tableKey = new Stack();
        private List<Table> resultTable = new List<Table>(); //вынес в глобальную переменную, чтобы слишком часто не передавать по значению(экономим немного памяти)
        List<Dictionary<string, List<string>>> rules;
        List<string> enterStrArr;

        //init
        public Runner(List<Dictionary<string, List<string>>> _rules) //правила берем из slr, они там уже есть.
        {
            rules = _rules;
        }

        //входная цепочка может состоять из подстрок и разделяется пробелами.
        //например: "abc" => "a b c d,e" |  "lot fok top lol,ik"
        public bool Convolution(List<Table> table, string enterString) //return true if success.
        {
            resultTable = table;
            List<string> enterStrArr = new List<string> (enterString.Split(' '));
            
            ProcessChain(enterStrArr);
            
            return true;
        }

        bool IsEqualKeys(List<string> key, string value)
        {
            string[] keyParams = MakeStringFromList(key).Split(':');
            return keyParams[0] == value;
        }

        //["id1", "id2"] -> "id1,id2";
        string MakeStringFromList(List<string> valueOfColumn)
        {
            string val = "";
            foreach (string elem in valueOfColumn)
            {
                val = (val == "") ? elem : (val + strSeparator + elem);
            }
            return val;
        }


        int GetClearKeyIndexFromTableWith(string value)
        {
            int counter = 0;
            while (!IsEqualKeys(resultTable[counter].key, value))
            {
                if (resultTable.Count > (counter + 1)) { counter++; } 
                else { return -1; }
            }
            return counter;
        }

        int GetSimpleKeyIndexFromTableWith(string value)
        {
            int counter = 0;
            while (!(MakeStringFromList(resultTable[counter].key) == value))
            {
                if (resultTable.Count > (counter + 1)) { counter++; }
                else { return -1; }
            }
            return counter;
        }

        int GetIndexFromValue(string value)
        {
            int counter = 0;
            while (resultTable[0].value[counter].columnOfTable != value)
            {
                if (resultTable.Count > (counter + 1)) { counter++; }
                else { return -1; }
            }
            return counter;
        }

        void ProcessChain(List<string> chain)
        {
            int counter = 0;
            //for (counter = 0; counter < chain.Count; counter++)
            //{
                int? i = GetClearKeyIndexFromTableWith(chain[counter]);
                if (i == null)
                {
                    //fatalErr
                    Console.WriteLine("\n--- [Fatal Error]: Element \"", chain[counter], "\" not found.\n");
                    return;
                }
                enterChain.Push(chain[counter]);
                if (counter + 1 < chain.Count)
                {
                    int indexOfNextVal = GetIndexFromValue(chain[counter + 1]);
                    string nextValueOfColumn = MakeStringFromList(resultTable[counter].value[indexOfNextVal].valueOfColumn);
                    if ((nextValueOfColumn == "RETURN") 
                        || (MakeStringFromList(resultTable[counter].value[resultTable[counter].value.Count].valueOfColumn) == "RETURN"))
                    {
                        TryToConvolutionInRule(counter);
                    } 
                    else if (nextValueOfColumn != "")
                    {
                        int indexOfSimpleKey = GetSimpleKeyIndexFromTableWith(nextValueOfColumn);
                        
                    }
                    else
                    {
                        //fatalErr
                        Console.WriteLine("\n--- [Fatal Error]: nextValueOfColumn is NULL(\"\"). Index of next val = ", indexOfNextVal, ";\n");
                    return;
                }
                }
                else if (MakeStringFromList(resultTable[counter].value[resultTable[counter].value.Count].valueOfColumn) == "RETURN") //подумать над концовкой
                {
                    TryToConvolutionInRule(counter);
                }
            //}
            return;
        }

        

        void TryToConvolutionInRule(int numberOfRule)
        {

        }
    }
}
