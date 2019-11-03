using System;
using System.Collections.Generic;
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
            RulesReader rulesReaeder = new RulesReader();
            Slr slr = new Slr(rulesReaeder.GetRules());
            var table = slr.GetTable();
            MakeAndLaunchRunner(table.rules, table.resultTable);
            Console.ReadLine();
		}
		
		private static void MakeAndLaunchRunner(List<Dictionary<string, List<string>>> rules, List<Table> resultTable)
		{
			string enterString = Console.ReadLine();
			if (enterString != "")
			{
				var runner = new Runner.Runner(rules); // при инициализации бегунка передаем правила из SLR.
				runner.Convolution(resultTable, enterString);
				while (runner.isSuccessfullyEnded == null)
				{//не совсем уверен в том, что это не тормозит выполнение работы в бегунке. В случае, если тормозит, то можно оставить блок просто пустым.
					System.Threading.Thread.Sleep(300);
				}
				bool runnerResult = runner.isSuccessfullyEnded ?? default(bool);
				//следуем дальнейшей логике...
			}
			else
			{
				Console.WriteLine("Входная строка пуста. Повторите ввод: ");
				MakeAndLaunchRunner(rules, resultTable);
			}
		}

	}
}