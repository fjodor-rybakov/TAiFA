using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Lexer
{
    public class Lexer
    {
        private readonly Controller _controller;

        public Lexer(params string[] paths)
        {
            _controller = new Controller(paths);
        }

        public List<LexerInfo> GetLexerInfo(string line, int numberString)
        {
            Dictionary<AutomateData, List<int>> acceptAutomates = InitAcceptAutomates();
            var value = "";
            var index = 0;
            var isString = false;
            var lexerInfo = new List<LexerInfo>();
            for (index = 0; index < line.Length; index++)
            {
                var ch = line[index];
                if (!isString)
                {
                    if (ch == ' ' || ch == '\t')
                    {
                        foreach (var automate in acceptAutomates)
                        {
                            if (automate.Value.Count == 0) continue;
                            if (!automate.Key.FinishStates.Contains(automate.Value.Last())) continue;
                            var isReserve = _controller.ReserveWords.Contains(value.ToLower());
                            lexerInfo.Add(new LexerInfo(value, TypeLexem.GetToken(automate.Key, automate.Value.Last()), isReserve, numberString, index));
                        }
                        value = "";
                        acceptAutomates = InitAcceptAutomates();
                    }
                    else if (_controller.MathWords.Contains(ch))
                    {
                        lexerInfo.Add(new LexerInfo(ch.ToString(), TypeLexem.MATH, false, numberString, index));
                        value = "";
                        acceptAutomates = InitAcceptAutomates();
                    }
                    else if (index < line.Length - 1 && CheckComparison(ch, line[index + 1], ref index, ref value))
                    {
                        lexerInfo.Add(new LexerInfo(value, TypeLexem.COMPARISON, false, numberString, index));
                        value = "";
                        acceptAutomates = InitAcceptAutomates();
                        continue;
                    }
                    else if (_controller.SplitSymbols.Contains(ch))
                    {
                        foreach (var automate in acceptAutomates)
                        {
                            if (automate.Value.Count == 0) continue;
                            if (!automate.Key.FinishStates.Contains(automate.Value.Last())) continue;
                            var isReserve = _controller.ReserveWords.Contains(value.ToLower());
                            lexerInfo.Add(new LexerInfo(value, TypeLexem.GetToken(automate.Key, automate.Value.Last()), isReserve, numberString, index));
                        }
                        lexerInfo.Add(new LexerInfo(ch.ToString(), TypeLexem.DELIMITER, false, numberString, index));
                        value = "";
                        acceptAutomates = InitAcceptAutomates();
                        if (lexerInfo.Count > 1 && lexerInfo[lexerInfo.Count - 2].Type == TypeLexem.TEXT)
                        {
                            continue;
                        }
                    }
                    else if (CheckOperation(ch))
                    {
                        lexerInfo.Add(new LexerInfo(ch.ToString(), TypeLexem.OPERATION, false, numberString, index));
                        value = "";
                    }
                    else
                    {
                        acceptAutomates = CheckLexem(ch, acceptAutomates, value);
                        if (acceptAutomates.Count == 0)
                        {
                            throw new Exception($"({numberString}, {index}): " + line[index] + " <--------- FAIL!!!");
                        }
                        value += ch;
                    }
                }
                else
                {
                    // обработка скобок (текста)
                    value += ch;
                    if (index + 1 < line.Length && line[index + 1] == '"')
                    {
                        isString = false;
                        lexerInfo.Add(new LexerInfo(value, TypeLexem.TEXT, false, numberString, index));
                        value = "";
                        acceptAutomates = InitAcceptAutomates();
                    }
                }
                
                if (ch == '"')
                {
                    isString = true;
                }

                //index++;
            }
            foreach (var automate in acceptAutomates)
            {
                if (automate.Value.Count == 0) continue;
                if (!automate.Key.FinishStates.Contains(automate.Value.Last())) continue;
                var isReserve = _controller.ReserveWords.Contains(value.ToLower());
                lexerInfo.Add(new LexerInfo(value, TypeLexem.GetToken(automate.Key, automate.Value.Last()), isReserve, numberString, index));
            }

            return lexerInfo;
        }

        private Dictionary<AutomateData, List<int>> CheckLexem(char ch, Dictionary<AutomateData, List<int>> acceptAutomates, string value)
        {
            var removeData = new List<AutomateData>();
            foreach(var item in acceptAutomates)
            {
                var currentState = item.Value.Count != 0 ? item.Value.Last() : 0;
                var command = ch.ToString();

                if (char.IsNumber(ch))
                {
                    command = "n";
                }
                else if (char.IsLetter(ch) && item.Key.Type == "identificator")
                {
                    command = "c";
                }
                
                if (item.Key.Type == "number2816")
                {
                    command = ch.ToString();
                    if (value.Length > 1)
                        if (value[1] == 'o' && CheckNum8(ch))
                            command = "on";
                        else if (value[1] == 'h' && CheckNum16(ch))
                            command = "hn";
                }

                if (item.Key.Automate.ContainsKey(command))
                {
                    currentState = item.Key.Automate[command][currentState];
                    if (currentState == -1)
                        removeData.Add(item.Key);
                    else if (currentState != -1)
                        acceptAutomates[item.Key].Add(currentState);
                }
                else
                    removeData.Add(item.Key);
            }

            RefreshAcceptAutomates(removeData, ref acceptAutomates);
            
            return acceptAutomates;
        }

        private bool CheckComparison(char firstCh, char lastCh, ref int index, ref string value)
        {
            if (_controller.ComparisonWords.Contains(string.Join("", firstCh, lastCh)))
            {
                value = string.Join("", firstCh, lastCh);
                index +=2;
                return true;
            }
            if (_controller.ComparisonWords.Contains(firstCh.ToString()))
            {
                value = firstCh.ToString();
                index++;
                return true;
            }
            return false;
        }


        private bool CheckOperation(char value)
        {
            return _controller.BindOptions.Contains(value);
        }

        private void RefreshAcceptAutomates(IEnumerable<AutomateData> removeData, ref Dictionary<AutomateData, List<int>> acceptAutomates)
        {
            foreach (var item in removeData)
            {
                acceptAutomates.Remove(item);
            }
        }
        
        private Dictionary<AutomateData, List<int>> InitAcceptAutomates()
        {
            var result = new Dictionary<AutomateData, List<int>>();
            foreach (var automateData in _controller.Automates)
                result.Add(automateData, new List<int>());
            
            return result;
        }
        
        private bool CheckNum16(char ch)
        {
            return ch >= 48 && ch <= 57 || ch >= 97 && ch <= 102 || ch == 'h';
        }

        private bool CheckNum8(char ch)
        {
            return ch >= 48 && ch <= 55 || ch == 'o';
        }
    }
}