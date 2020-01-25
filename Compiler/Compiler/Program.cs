using System;
using System.Collections.Generic;
using System.IO;
using Compiler.Lexer;
using Compiler.SLR;

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
			var lexerData = MakeLexer();
			
            var rulesReader = new RulesReader();
            var rules = rulesReader.GetRules();
            var slr = new Slr(rules);
            var table = slr.GetTable();
			MakeAndLaunchRunner(table.rules, table.resultTable, lexerData);
            
            
            Console.ReadLine();
		}

		private static List<LexerInfo> MakeLexer()
		{
			var reader = new StreamReader(PATH_DATA);
			var lexer = new Lexer.Lexer(PATH_IDENTIFICATOR, PATH_NUMBER10, PATH_NUMBER2816);
			string line;
			var lexerData = new List<LexerInfo>();
			int numberString = 0;
			while ((line = reader.ReadLine()) != null)
			{
				var lexerInfo = lexer.GetLexerInfo(line, numberString);
				foreach (var item in lexerInfo)
				{
					Console.WriteLine("Value: " + item.Value + " => Type: " + item.Type + ", IsReserve: " + item.IsReserve);
					lexerData.Add(item);
				}
				numberString++;
			}
			return lexerData;
		}

		private static void MakeAndLaunchRunner(List<Dictionary<HeadOfRule, List<string>>> rules, List<Table> resultTable, List<LexerInfo> lexerData)
		{
			var runner = new Runner.Runner(rules); // при инициализации бегунка передаем правила из SLR.
			runner.Convolution(resultTable, lexerData);
			while (runner.isSuccessfullyEnded == null)
			{//не совсем уверен в том, что это не тормозит выполнение работы в бегунке. В случае, если тормозит, то можно оставить блок просто пустым.
				System.Threading.Thread.Sleep(300);
			}
			bool runnerResult = runner.isSuccessfullyEnded ?? default(bool);
			// Console.WriteLine($"Runner result: {runnerResult}");
			//следуем дальнейшей логике...
		}

	}
}