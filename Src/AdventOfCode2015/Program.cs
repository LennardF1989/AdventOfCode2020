using System;
using AdventOfCode.Shared;
using AdventOfCode2015.Days;

namespace AdventOfCode2015
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;

            Day01.StartA();
            Day01.StartB();

            Day02.StartA();
            Day02.StartB();

            Logger.ShowDebug = true;

            Day03.StartA();
            Day03.StartB();

            Console.ReadKey();
        }
    }
}
