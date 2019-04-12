using System;
using System.Collections.Generic;
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
				List<LexerInfo> lexerInfo = lexer.GetLexerInfo(line);
				foreach (var item in lexerInfo)
				{
					Console.WriteLine("Value: " + item.Value + " => Type: " + item.Type + ", IsReserve: " + item.IsReserve);
				}
			}
		}
	}
}