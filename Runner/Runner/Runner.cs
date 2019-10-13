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
        private Stack enterChain = new Stack();
        private Stack tableKey = new Stack();
        private List<Table> resultTable = new List<Table>(); //вынес в глобальную переменную, чтобы слишком часто не передавать по значению(экономим немного памяти)

        //входная цепочка может состоять из подстрок и разделяется пробелами.
        //например: "abc" => "a b c d,e" |  "lot fok top lol,ik"
        public bool Convolution(List<Table> table, String enterString) //return true if success.
        {
            resultTable = table;
            List<string> enterStrArr = new List<string> (enterString.Split(' '));
            int ctr;
            for (ctr = 0; ctr < enterStrArr.Count; ctr++)
            {
                int? i = GetIndexFromTableWith(enterStrArr[ctr]);
                if (i != null) {
                    enterChain.Push(enterStrArr[ctr]);
                    
                } 
                else
                {
                    //fatalErr
                }
                
            }
            return true;
        }

        //["id1", "id2"] -> "id1,id2";
        //далее происходит сравнение строк.
        bool IsEqualKeys(List<string> key, string value)
        {
            string keyVal = "";
            foreach (string id in key)
            {
                if (keyVal != "") { keyVal += "," + id; }
                else { keyVal = id; }
            }
            return keyVal == value;
        }


        int? GetIndexFromTableWith(string value)
        {
            int ctr = 0;
            while (!IsEqualKeys(resultTable[ctr].key, value))
            {
                if (resultTable.Count > (ctr + 1)) { ctr++; } 
                else { return null; }
            }
            return ctr;
        }
    }
}
