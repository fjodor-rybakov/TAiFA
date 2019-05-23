using System;
using System.IO;

namespace Compiler
{
	internal class Program
	{
		private const string PATH_DATA = @"../../../files/lexer/data.txt";
		private const string PATH_IDENTIFICATOR = @"../../../files/lexer/identificator.txt";
		private const string PATH_NUMBER10 = @"../../../files/lexer/number10.txt";
		private const string PATH_NUMBER2816 = @"../../../files/lexer/number2816.txt";
		
		public static void Main()
		{
			var reader = new StreamReader(PATH_DATA);
			var lexer = new Lexer.Lexer(PATH_IDENTIFICATOR, PATH_NUMBER10, PATH_NUMBER2816);
			
			var generator = new Generator.Generator();

			/*string line;
			while ((line = reader.ReadLine()) != null)
			{
				var lexerInfo = lexer.GetLexerInfo(line);
				foreach (var item in lexerInfo)
				{
					Console.WriteLine("Value: " + item.Value + " => Type: " + item.Type + ", IsReserve: " + item.IsReserve);
				}
				var recDown = new RecDown.RecDown(lexerInfo);
				Console.WriteLine("Var is valid: " + recDown.CheckVar());
			}*/
		}
	}
}