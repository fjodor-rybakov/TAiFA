﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.SLR;
using Compiler.Lexer;

namespace Compiler.Runner
{
    struct Value
    {
        public string columnOfTable; // элемент сверху
        public List<string> valueOfColumn; // значения этого элменета
    }

    class Runner
    {
        private string strSeparator = ",";
        private Stack<string> enterChain = new Stack<string>();
        private List<Table> resultTable = new List<Table>(); //вынес в глобальную переменную, чтобы слишком часто не передавать по значению(экономим немного памяти)
        private bool firstEnter = true;

        public bool? isSuccessfullyEnded = null;
        public List<Dictionary<string, List<string>>> rules;
        public List<LexerInfo> lexerData;

        //init
        public Runner(List<Dictionary<string, List<string>>> _rules) //правила берем из slr, они там уже есть.
        {
            rules = _rules;
        }

        //входная цепочка может состоять из подстрок и разделяется пробелами.
        //например: "abc" => "a b c d,e" |  "lot fok top lol,ik"
        public void Convolution(List<Table> table, List<LexerInfo> lexerChainData) //return true if success.
        {
            resultTable = table;
            lexerData = lexerChainData;
            
            ProcessChain();
        }

        void ProcessChain()
        {
            int counter = 0;
            string firstElement = "";
            if (!firstEnter)
            {
                if (enterChain.Count != 0)
                {
                    firstElement = MakeStringFromList(resultTable[GetSafeKeyIndexFromTableWith(enterChain.Peek())].value[GetColumnIndexFromValue(lexerData[counter].Value)].valueOfColumn);
                }
                else
                {
                    //fatalErr
                    Console.WriteLine("\n--- [Fatal Error]: Enter Chain is Empty!\n");
                    isSuccessfullyEnded = false;
                    return;
                }
            }
            else
            {
                int valIndex = GetColumnIndexFromValue(lexerData[counter].Value);
                var value = resultTable[counter].value[valIndex].valueOfColumn;
                firstElement = MakeStringFromList(value);
                firstEnter = false;
            }

            if (firstElement == "")
            {
                //fatalErr
                Console.WriteLine("\n--- [Fatal Error]: Element \"", lexerData[counter].Value, "\" not found.\n");
                isSuccessfullyEnded = false;
                return;
            }
            enterChain.Push(firstElement);
            RecursiveAnalizingForChainWith(counter);
            return;
        }

        string GetClearKey(string dirtyKey)
        {
            string[] keyParams = dirtyKey.Split(':');
            return keyParams[0];
        }

        bool IsEqualKeys(List<string> key, string value)
        {
            return GetClearKey(MakeStringFromList(key)) == value;
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

        int GetSafeKeyIndexFromTableWith(string value)
        {
            int counter = 0;
            while (!(MakeStringFromList(resultTable[counter].key) == value))
            {
                if (resultTable.Count > (counter + 1)) { counter++; }
                else { return -1; }
            }
            return counter;
        }

        int GetColumnIndexFromValue(string value)
        {
            int counter = 0;
            while (resultTable[0].value[counter].columnOfTable != value)
            {
                if (resultTable.Count > (counter + 1)) { counter++; }
                else { return -1; }
            }
            return counter;
        }

        int GetNumberOfRule(string key)
        {
            string[] keyParams = key.Split(':');
            if (keyParams.Length > 1)
            {
                return int.Parse(keyParams[1]);
            }
            return -1;
        }

        

        void RecursiveAnalizingForChainWith(int counter)
        {
            if (counter + 1 < lexerData.Count)
            {
                int columnIndexOfNextVal = GetColumnIndexFromValue(lexerData[counter + 1].Value);
                string nextValueOfColumn = MakeStringFromList(resultTable[counter].value[columnIndexOfNextVal].valueOfColumn);
                if ((nextValueOfColumn == "RETURN")
                    || (MakeStringFromList(resultTable[counter].value[resultTable[counter].value.Count].valueOfColumn) == "RETURN"))
                {
                    TryToConvolutionInRule(GetNumberOfRule(enterChain.Peek()));
                }
                else if (nextValueOfColumn != "")
                {
                    int indexOfSafeKey = GetSafeKeyIndexFromTableWith(nextValueOfColumn);
                    if (indexOfSafeKey == -1) //Проверяем, есть ли такой элемент в ключах таблицы
                    {
                        //fatalErr
                        Console.WriteLine("\n--- [Fatal Error]: indexOfSafeKey == -1 (not exist). For value: ", nextValueOfColumn, ";\n");
                        isSuccessfullyEnded = false;
                        return;
                    }
                    enterChain.Push(nextValueOfColumn);
                    RecursiveAnalizingForChainWith(++counter);
                }
                else
                {
                    //fatalErr
                    Console.WriteLine("\n--- [Fatal Error]: nextValueOfColumn is NULL(\"\"). Index of next val = ", columnIndexOfNextVal, ";\n");
                    isSuccessfullyEnded = false;
                    return;
                }
            }
            else if (MakeStringFromList(resultTable[counter].value[resultTable[counter].value.Count].valueOfColumn) == "RETURN") //подумать над концовкой
            {
                Console.WriteLine("Найден конец цепочки...");
                TryToConvolutionInRule(GetNumberOfRule(enterChain.Peek()));
            }
            else
            {
                //fatalErr
                Console.WriteLine("\n--- [Fatal Error]: Last element: \"", resultTable[counter].value[resultTable[counter].value.Count].valueOfColumn, "\" != RETURN.\n");
                isSuccessfullyEnded = false;
                return;
            }
        }

        void TryToConvolutionInRule(int numberOfRule)
        {
            //Пробуем свернуть. Получается -> идем дальше; нет - завершаем с ошибкой.
            string key = rules[numberOfRule].Keys.ElementAt(0);
            List<string> rule = rules[numberOfRule] [rules[numberOfRule].Keys.ElementAt(0)];
            for (int i = rule.Count; i == 0; i--)
            {
                if (rule[i] == GetClearKey(enterChain.Peek()))
                {
                    enterChain.Pop();
                }
                else
                {
                    //fatalErr
                    Console.WriteLine("\n--- [Fatal Error]: Clear Key: ", GetClearKey(enterChain.Peek()), " - not conform to rule. Rule element: ", rule[i], "; in rule ", numberOfRule, "; \n");
                    isSuccessfullyEnded = false;
                    return;
                }
            }
            
            RebuildAndCheckChain(key, rule.Count);
        }

        void RebuildAndCheckChain(string key, int countElementsInRule)
        {
            lexerData.RemoveRange(0, countElementsInRule);
            lexerData.Insert(0, new LexerInfo(key, "?", false));
            if ((lexerData.Count == 1) && (enterChain.Count == 0))
            {
                //finish
                Console.WriteLine("Свертка завершена успешно. Начальный элемент: ", key);
                isSuccessfullyEnded = true;
                return;
            }
            else
            {
                ProcessChain();
            }
        }
    }
}