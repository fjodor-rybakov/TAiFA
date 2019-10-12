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
        Stack enterChain = new Stack();
        Stack tableKey = new Stack();

        //входная цепочка может состоять из подстрок и разделяется пробелами.
        //например: "abc" => "a b c" |  "lot fok top"
        void runner(List<Table> table, String enterString)
        {
            string [] enterStrArr = enterString.Split(' ');
            foreach (string element in enterStrArr)
            {
                
            }
        }

    }
}
