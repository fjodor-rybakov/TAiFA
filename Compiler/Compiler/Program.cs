using System;
using System.IO;

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
			
			var reader = new StreamReader(pathData);
			var lexer = new Lexer.Lexer(pathIdentificator, pathNumber10, pathNumber2816);
			
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				var lexerInfo = lexer.GetLexerInfo(line);
				foreach (var item in lexerInfo)
				{
					Console.WriteLine("Value: " + item.Value + " => Type: " + item.Type + ", IsReserve: " + item.IsReserve);
				}
				var recDown = new RecDown.RecDown(lexerInfo);
				Console.WriteLine("Var is valid: " + recDown.CheckVar());
			}
		}
	}
}