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
        public List<string> key; // единственный ключ, в котором может быть несколько элементов
        public List<Value> value; // сама таблица

    }

    class Runner
    {
        private string strSeparator = ",";
        private Stack enterChain = new Stack();
        private Stack tableKey = new Stack();
        private List<Table> resultTable = new List<Table>(); //вынес в глобальную переменную, чтобы слишком часто не передавать по значению(экономим немного памяти)
        List<Dictionary<string, List<string>>> rules; //правила берем из slr, они там уже есть.


        public Runner(List<Dictionary<string, List<string>>> _rules)
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
            return MakeStringFromList(key) == value;
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


        int? GetKeyIndexFromTableWith(string value)
        {
            int ctr = 0;
            while (!IsEqualKeys(resultTable[ctr].key, value))
            {
                if (resultTable.Count > (ctr + 1)) { ctr++; } 
                else { return null; }
            }
            return ctr;
        }

        int? GetIndexFromValue(string value)
        {
            int ctr = 0;
            while (resultTable[0].value[ctr].columnOfTable != value)
            {
                if (resultTable.Count > (ctr + 1)) { ctr++; }
                else { return null; }
            }
            return ctr;
        }

        void ProcessChain(List<string> chain)
        {
            int ctr;
            for (ctr = 0; ctr < chain.Count; ctr++)
            {
                int? i = GetKeyIndexFromTableWith(chain[ctr]);
                if (i == null)
                {
                    //fatalErr
                    Console.WriteLine("\n--- [Fatal Error]: Element \"", chain[ctr], "\" not found.\n");
                    break;
                }
                enterChain.Push(chain[ctr]);
                if (ctr + 1 < chain.Count)
                {
                    int? indexOfNextVal = GetIndexFromValue(chain[ctr + 1]);
                    int idNext = indexOfNextVal ?? -1;
                    if ((MakeStringFromList(resultTable[ctr].value[idNext].valueOfColumn) == "RETURN") 
                        || (MakeStringFromList(resultTable[ctr].value[resultTable[ctr].value.Count].valueOfColumn) == "RETURN"))
                    {
                        TryToConvolutionInRule(ctr);
                    }
                }
                else if (MakeStringFromList(resultTable[ctr].value[resultTable[ctr].value.Count].valueOfColumn) == "RETURN") //подумать над концовкой
                {
                    TryToConvolutionInRule(ctr);
                }
            }
        }

        

        void TryToConvolutionInRule(int numberOfRule)
        {

        }
    }
}
