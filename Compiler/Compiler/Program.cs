using System;
using System.IO;
using Compiler.LLSyntaxer;
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
            SLR.Slr slr = new SLR.Slr(rulesReaeder.GetRules());
            slr.GetTable();
            Console.ReadLine();
		}
	}
}