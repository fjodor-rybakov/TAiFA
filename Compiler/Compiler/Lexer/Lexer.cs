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
            foreach (var ch in line)
            {
                if (!isString)
                {
                    if (ch == ' ')
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
                    else if (CheckOperation(ch) && index + 1 < line.Length && !char.IsNumber(line[index + 1]))
                    {
                        lexerInfo.Add(new LexerInfo(value, TypeLexem.OPERATION, false, numberString, index));
                        value = "";
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
                        if (lexerInfo[lexerInfo.Count - 2].Type == TypeLexem.TEXT)
                        {
                            continue;
                        }
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

                index++;
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