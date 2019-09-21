using System;
using System.Collections.Generic;

namespace SLR
{
    class Program
    {
        static void Main(string[] args)
        {
            var finder = new DirSetFinder();
            Slr slr = new Slr(finder.GetRules());
            slr.analyze();
        }
    }
}
