using System;
using System.IO;
using Compiler.Lexer1;

namespace Compiler
{
	internal class Program
	{
		public static void Main()
		{
			const string pathData = @"C:/TAiFA/Compiler/files/lexer/data.txt";
			const string pathIdentificator = @"C:/TAiFA/Compiler/files/lexer/identificator.txt";
			const string pathNumber10 = @"C:/TAiFA/Compiler/files/lexer/number10.txt";
			const string pathNumber2816 = @"C:/TAiFA/Compiler/files/lexer/number2816.txt";
			
			StreamReader reader = new StreamReader(pathData);
			Lexer1.Lexer lexer = new Lexer1.Lexer(pathIdentificator, pathNumber10, pathNumber2816);
			
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				// LexerInfo lexerInfo = lexer.GetLexerInfo(line);
				lexer.GetLexerInfo(line);
			}

			/*Controller controller = new Controller(pathIdentificator, pathNumber10, pathNumber2816);

			foreach (var automates in controller.Automates)
			{
				foreach (var item in automates.Value.Automate)
				{
					Console.WriteLine("Key: " + item.Key + " Value: " + string.Join(", ", item.Value));	
				}

				Console.WriteLine();
			}*/

			/*const string pathData = @"C:/TAiFA/Compiler/files/data.txt";
			const string pathIdentificator = @"C:/TAiFA/Compiler/files/identificator.txt";
			const string pathNumber10 = @"C:/TAiFA/Compiler/files/number10.txt";
			const string pathNumber2816 = @"C:/TAiFA/Compiler/files/number2816.txt";
			const string test = @"C:/TAiFA/Compiler/files/test.txt";

			Interpreter interpreter = new Interpreter(pathIdentificator, pathNumber10, pathNumber2816);

			StreamReader dataReader = new StreamReader(pathData);
			Lexer.Lexer lexer = new Lexer.Lexer(interpreter);

			string line;
			string saveLine = "";
			while ((line = dataReader.ReadLine()) != null)
			{
				
				var result = lexer.checkLexer(line, ref interpreter);
				foreach (var item in result)
				{
					Console.WriteLine(item.Key + item.Value + ", " + " IS_RESERVE: " + lexer.isReserve(item.Value));
				}
				
			}*/
		}
	}
}