using System;

namespace Compiler.Helper
{
    public static class Helper
    {
        public static void PrintDelimiter(int count)
        {
            for (var i = 0; i < count; i++)
            {
                Console.Write("-");
            }

            Console.WriteLine();
        }
    }
}