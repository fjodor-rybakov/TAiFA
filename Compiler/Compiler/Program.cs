using System;
using System.IO;
using Compiler.Lexer;

namespace Compiler
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			const string pathData = @"C:/TAiFA/Compiler/files/data.txt";
			const string pathIdentificator = @"C:/TAiFA/Compiler/files/identificator.txt";
			const string pathNumber10 = @"C:/TAiFA/Compiler/files/number10.txt";
			const string pathNumber2816 = @"C:/TAiFA/Compiler/files/number2816.txt";
			
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
				
			}
		}
	}
}