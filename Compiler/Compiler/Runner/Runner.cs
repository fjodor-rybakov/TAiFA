using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.SLR;
using Compiler.Lexer;

namespace Compiler.Runner
{
    class Runner
    {
        private string strSeparator = ",";
        private Stack<string> enterChain = new Stack<string>();
        private List<Table> resultTable = new List<Table>(); //вынес в глобальную переменную, чтобы слишком часто не передавать по значению(экономим немного памяти)
        private bool firstEnter = true;

        public bool? isSuccessfullyEnded = null;
        public List<Dictionary<string, List<string>>> rules;
        public List<LexerInfo> lexerData;
        int commonCounter = 0;
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
            string firstElement = "";
            if (!firstEnter)
            {
                int tableStrIndex = GetSafeKeyIndexFromTableWith(enterChain.Peek());
                int columnValueIndex = GetColumnIndexFromValue(lexerData[commonCounter].Value);
                firstElement = MakeStringFromList(resultTable[tableStrIndex].value[columnValueIndex].valueOfColumn);
            }
            else
            {
                string word = (!lexerData[commonCounter].IsReserve && 
                              (lexerData[commonCounter].Type == TypeLexem.IDENTIFICATOR || 
                              lexerData[commonCounter].Type == TypeLexem.TEXT || 
                              lexerData[commonCounter].Type == TypeLexem.MATH || 
                              lexerData[commonCounter].Type == TypeLexem.COMPARISON ||
                              lexerData[commonCounter].Type == TypeLexem.NUMBER10
                              ))
                    ? lexerData[commonCounter].Type 
                    : lexerData[commonCounter].Value;
                int columnIndexOfNextVal = GetColumnIndexFromValue(word);
                if (columnIndexOfNextVal == -1)
                {
                    //fatalErr
                    PrintEndOfProgram("\n--- [Fatal Error]: индекс колонки таблицы не найден для значения: \"" + lexerData[commonCounter].Value + "\";\n", false);
                    return;
                }
                var value = resultTable[commonCounter].value[columnIndexOfNextVal].valueOfColumn;
                firstElement = MakeStringFromList(value);
                enterChain.Push(MakeStringFromList(resultTable[0].key));
                firstEnter = false;
            }

            if (firstElement == "")
            {
                //fatalErr
                PrintEndOfProgram("\n--- [Fatal Error]: Элемент: \"" + lexerData[commonCounter].Value + "\" не был найден в таблице.\n", false);
                return;
            }
            enterChain.Push(firstElement);
            RecursiveAnalizingForChainWith(commonCounter);
            return;
        }

        string GetClearKey(string dirtyKey)
        {
            string[] keyParams = dirtyKey.Split('^');
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
                string  str = MakeStringFromList(resultTable[counter].key);
                if (resultTable.Count > (counter + 1)) { counter++; }
                else { return -1; }
            }
            //string stra = MakeStringFromList(resultTable[counter].key);
            //Console.WriteLine("+++ Safe key = " + stra + "; From value = " + value + ";\n");
            return counter;
        }

        int GetColumnIndexFromValue(string value)
        {
            int counter = 0;
            while (resultTable[0].value[counter].columnOfTable != value)
            {
                if (resultTable[0].value.Count > (counter + 1)) { counter++; }
                else { return -1; }
            }
            //string equalValues = "+++ Column index found: " + counter + "; " + resultTable[0].value[counter].columnOfTable + " == " + value;
            //Console.WriteLine(equalValues);
            return counter;
        }

        int GetNumberOfRule(string key)
        {
            string[] keyParams = key.Split('^');
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
                string word = (!lexerData[counter + 1].IsReserve && 
                               (lexerData[counter + 1].Type == TypeLexem.IDENTIFICATOR || 
                               lexerData[counter + 1].Type == TypeLexem.TEXT || 
                               lexerData[counter + 1].Type == TypeLexem.COMPARISON || 
                               lexerData[counter + 1].Type == TypeLexem.MATH ||
                               lexerData[counter + 1].Type == TypeLexem.NUMBER10
                               )) 
                    ? lexerData[counter + 1].Type
                    : lexerData[counter + 1].Value;
                //Console.WriteLine("Следующее значение лексера: " + lexerData[counter + 1].Value + "; его тип: " + lexerData[counter + 1].Type);
                int columnIndexOfNextVal = GetColumnIndexFromValue(word);
                if (columnIndexOfNextVal == -1)
                {
                    //fatalErr
                    PrintEndOfProgram("\n--- [Fatal Error]: индекс колонки таблицы не найден для значения: \"" + lexerData[counter + 1].Value + "\";\n", false);
                    return;
                }
                int indexStr = GetSafeKeyIndexFromTableWith(enterChain.Peek());
                string nextValueOfColumn = MakeStringFromList(resultTable[indexStr].value[columnIndexOfNextVal].valueOfColumn);

                if (nextValueOfColumn == "RETURN")
                {
                    TryToConvolutionInRule(GetNumberOfRule(enterChain.Peek()), (counter + 1));
                }
                else if (nextValueOfColumn != "")
                {
                    int indexOfSafeKey = GetSafeKeyIndexFromTableWith(nextValueOfColumn);
                    if (indexOfSafeKey == -1) //Проверяем, есть ли такой элемент в ключах таблицы
                    {
                        //fatalErr
                        PrintEndOfProgram("\n--- [Fatal Error]: ключ таблицы не найден для значения: " + nextValueOfColumn + "; resultTable[" + indexStr + "].value[" + columnIndexOfNextVal + "]ы\n", false);
                        return;
                    }
                    enterChain.Push(nextValueOfColumn);
                    RecursiveAnalizingForChainWith(++counter);
                }
                else
                {
                    //fatalErr
                    PrintEndOfProgram("\n--- [Fatal Error]: resultTable[" + indexStr + "].value[" + columnIndexOfNextVal+ "].valueOfColumn == EMPTY;\nПереходить дальше некуда. Смотри SLR или правила.", false);
                    return;
                }
            }
            else
            {
                //Console.WriteLine("Найден конец цепочки...");
                TryToConvolutionInRule(GetNumberOfRule(enterChain.Peek()), ++counter);
            }
        }

        void TryToConvolutionInRule(int numberOfRule, int lexerCounter)
        {
            //Пробуем свернуть. Получается -> идем дальше; нет - завершаем с ошибкой.
            string key = rules[numberOfRule].Keys.ElementAt(0);
            List<string> rule = rules[numberOfRule] [rules[numberOfRule].Keys.ElementAt(0)];
            for (int i = rule.Count - 1; i >= 0; i--)
            {
                //Console.WriteLine("///in loop with index: " + i);
                if ((rule[i] == GetClearKey(enterChain.Peek())) && (enterChain.Count >= 1))
                {
                    enterChain.Pop();
                }
                else
                {
                    //fatalErr
                    PrintEndOfProgram("\n--- [Fatal Error]: Clear Key: " + GetClearKey(enterChain.Peek()) + " - not conform to rule. Rule element: " + rule[i] + "; in rule " + numberOfRule + "; EnterChain.Count ==" + enterChain.Count + "\n", false);
                    return;
                }
            }
            Console.WriteLine(" |||| Свертка по правилу №" + numberOfRule + "; Rule Key: " + key + ";\n");
            RebuildAndCheckChain(key, rule.Count, lexerCounter);
        }

        void RebuildAndCheckChain(string key, int countElementsInRule, int lexerCounter)
        {
            if (lexerData.Count < countElementsInRule)
            {
                //fatalErr
                PrintEndOfProgram("\n--- [Fatal Error]: Недостаточно элементов для свертки в правило.\n", false);
                return;
            }
            lexerData.RemoveRange(lexerCounter - countElementsInRule, countElementsInRule);
            // TODO line and pos
            lexerData.Insert(lexerCounter - countElementsInRule, new LexerInfo(key, "?", false, 0, 0));
            commonCounter = lexerCounter - countElementsInRule;
            if ((lexerData.Count == 1) && (enterChain.Count == 1))
            {
                //finish
                PrintEndOfProgram("Свертка завершена успешно. Начальный элемент: " + key, true);
                return;
            }
            else
            {
                ProcessChain();
            }
        }

        void PrintEndOfProgram(string text, bool success)
        {
            Console.WriteLine(text);
            isSuccessfullyEnded = success;
        }
    }
}