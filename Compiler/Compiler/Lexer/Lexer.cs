using System;
using System.Collections.Generic;

namespace Compiler.Lexer
{
    public class Lexer
    {
        private Dictionary<string, List<int>> _identificator;
        private List<string> resWords = new List<string>
        {
            "begin", "end", "main", "number", "decimal", "string", "char", "bool", "number2", "number8", "number216", "for", "while", "if", "else" 
        };
        
        public Lexer(Interpreter interpreter)
        {
            _identificator = interpreter.Automates["identificator"].Automate;
        }
        
        public string getType(char ch)
        {
            
            if (Char.IsLetter(ch))
            {
                return "c";
            }

            if (Char.IsNumber(ch))
            {
                return "n";
            }

            return ch.ToString();
        }

        public List<KeyValuePair<string, string>> checkLexer(string line, ref Interpreter interpreter)
        {
	        List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
	        string saveLine = "";
	        try
	        {
		        var identificator = interpreter.Automates["identificator"].Automate;
		        var number10 = interpreter.Automates["number10"].Automate;
		        var number2816 = interpreter.Automates["number2816"].Automate;
		        
		       
	            int counterLength = 0, lastState = 0;
				bool isAcceptIdent = false,
					isAcceptNumber = false,
					isAcceptNumber2 = false,
					isAcceptNumber8 = false,
					isAcceptNumber16 = false;
	
				for (int i = 0; i < line.Length; i++)
				{
					Char ch = line[i];
	
					if (checkOption(ch, line, i))
					{
						// Console.WriteLine("OPTION: " + ch);
						result.Add(new KeyValuePair<string, string>("OPTION: ", ch.ToString()));
						continue;
					}
	
					if (ch == '*' || ch == '/' || ch == '=')
					{
						//Console.WriteLine("OPTION: " + ch);
						result.Add(new KeyValuePair<string, string>("OPTION: ", ch.ToString()));
					}
					if (ch == ' ')
					{
						counterLength = 0;
					}
					else
					{
						if (counterLength == 0)
						{
							if (isAcceptIdent)
							{
								//Console.WriteLine("ID: " + saveLine);
								result.Add(new KeyValuePair<string, string>("ID: ", saveLine));
							}
							if (isAcceptNumber)
							{
								if (lastState >= 3)
								{
									//Console.WriteLine("DECIMAL: " + saveLine);
									result.Add(new KeyValuePair<string, string>("DECIMAL: ", saveLine));
								}
								else
								{
									//Console.WriteLine("NUMBER: " + saveLine);
									result.Add(new KeyValuePair<string, string>("NUMBER: ", saveLine));
								}
							}
							
							if (isAcceptNumber2)
							{
								//Console.WriteLine("NUMBER2: " + saveLine);
								result.Add(new KeyValuePair<string, string>("NUMBER2: ", saveLine));
							}
							
							if (isAcceptNumber8)
							{
								//Console.WriteLine("NUMBER8: " + saveLine);
								result.Add(new KeyValuePair<string, string>("NUMBER8: ", saveLine));
							}
						
							if (isAcceptNumber16)
							{
								//Console.WriteLine("NUMBER16: " + saveLine);
								result.Add(new KeyValuePair<string, string>("NUMBER16: ", saveLine));
							}
							
							isAcceptIdent = false;
							isAcceptNumber = false;
							isAcceptNumber2 = false;
							isAcceptNumber8 = false;
							isAcceptNumber16 = false;
	
							if (counterLength == 0 && (Char.IsLetter(ch) || ch == '_'))
							{
								isAcceptIdent = true;
								lastState = 0;
								saveLine = "";
							}
							if (counterLength == 0 && (Char.IsNumber(ch) || ch == '+' || ch == '-'))
							{
								isAcceptNumber = true;
								lastState = 0;
								saveLine = "";
							}
						}
						else if (ch == 'b' && line[i - 1] == '0')
						{
							isAcceptIdent = false;
							isAcceptNumber = false;
							isAcceptNumber8 = false;
							isAcceptNumber16 = false;
							isAcceptNumber2 = true;
						}
						else if ( ch == 'o' && line[i - 1] == '0')
						{
							isAcceptIdent = false;
							isAcceptNumber = false;
							isAcceptNumber16 = false;
							isAcceptNumber2 = false;
							isAcceptNumber8 = true;
						}
						else if (ch == 'h' && line[i - 1] == '0')
						{
							isAcceptIdent = false;
							isAcceptNumber = false;
							isAcceptNumber2 = false;
							isAcceptNumber8 = false;
							isAcceptNumber16 = true;
						}
						
						if (isAcceptIdent)
						{
							lastState = identificator[getType(ch)][lastState];
							saveLine += ch;
						}
	
						if (isAcceptNumber2)
						{
							lastState = 2;
							lastState = number2816[ch.ToString()][lastState];
							saveLine += ch;
						}
	
						if (isAcceptNumber8)
						{
							lastState = 4;
							lastState = number2816[convertNum8(ch)][lastState];
							saveLine += ch;
						}
						
						if (isAcceptNumber16)
						{
							lastState = 6;
							saveLine += ch;
							lastState = number2816[convertNum16(ch)][lastState];
						}
						
						if (isAcceptNumber)
						{
							saveLine += ch;
							lastState = number10[convertNum(ch)][lastState];
						}
	
						counterLength++;
					}
	
					if (i == line.Length - 1)
					{
						if (isAcceptIdent && !isFinishState(interpreter.Automates["identificator"].FinishStates, lastState))
						{
							throw new Exception("Ошибка синтаксиса");
						}
						
						if (isAcceptNumber && !isFinishState(interpreter.Automates["number10"].FinishStates, lastState))
						{
							throw new Exception("Ошибка синтаксиса");
						}
						
						if (isAcceptNumber2 && !isFinishState(interpreter.Automates["number2816"].FinishStates, lastState))
						{
							throw new Exception("Ошибка синтаксиса");
						}
						
						if (isAcceptNumber8 && !isFinishState(interpreter.Automates["number2816"].FinishStates, lastState))
						{
							throw new Exception("Ошибка синтаксиса");
						}
						
						if (isAcceptNumber16 && !isFinishState(interpreter.Automates["number2816"].FinishStates, lastState))
						{
							throw new Exception("Ошибка синтаксиса");
						}
						
						if (isAcceptIdent)
						{
							//Console.WriteLine("ID: " + saveLine);
							result.Add(new KeyValuePair<string, string>("ID: ", saveLine));
						}
						
						if (isAcceptNumber)
						{
							if (lastState >= 3)
							{
								//Console.WriteLine("DECIMAL: " + saveLine);
								result.Add(new KeyValuePair<string, string>("DECIMAL: ", saveLine));
							}
							else
							{
								//Console.WriteLine("NUMBER: " + saveLine);
								result.Add(new KeyValuePair<string, string>("NUMBER: ", saveLine));
							}
						}
						
						if (isAcceptNumber2)
						{
							//Console.WriteLine("NUMBER2: " + saveLine);
							result.Add(new KeyValuePair<string, string>("NUMBER2: ", saveLine));
						}
						
						if (isAcceptNumber8)
						{
							//Console.WriteLine("NUMBER8: " + saveLine);
							result.Add(new KeyValuePair<string, string>("NUMBER8: ", saveLine));
						}
						
						if (isAcceptNumber16)
						{
							//Console.WriteLine("NUMBER16: " + saveLine);
							result.Add(new KeyValuePair<string, string>("NUMBER16: ", saveLine));
						}
	
						counterLength = 0;
						lastState = 0;
						saveLine = "";
					}
				}
	
				
	        }
	        catch (Exception)
	        {
		        Console.WriteLine("Ошибка синтаксиса: " + saveLine);
	        }
	        
	        return result;
        }

        private bool isFinishState(List<int> states, int lastState)
        {
            foreach (var state in states)
            {
                if (state == lastState)
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkOption(Char ch, string line, int i)
        {
            if (ch == '+' || ch == '-')
            {
                try
                {
                    if (line[i + 1] == ' ')
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }

            return false;
        }

        private string convertNum16(char ch)
        {
	        string newType = getType(ch);
	        if (ch >= 48 && ch <= 57 || ch >= 97 && ch <= 102 || ch == 'h')
	        {
		        newType = "hn";
	        }

	        return newType;
        }

        private string convertNum8(Char ch)
        {
	        string newType = getType(ch);
	        if (ch >= 48 && ch <= 55 || ch == 'o')
	        {
		        newType = "on";
	        }

	        return newType;
        }

        private string convertNum(Char ch)
        {
	        string newType = getType(ch);
	        if (ch == 'e')
	        {
		        newType = "e";
	        }

	        return newType;
        }

        public bool isReserve(string value)
        {
	        return resWords.IndexOf(value.ToLower()) != -1;
        }
    }
}