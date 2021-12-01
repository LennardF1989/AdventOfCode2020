using System;
using AdventOfCode2021.Days;

namespace AdventOfCode2021
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;

            Day01.StartA();
            Day01.StartB();

            Logger.ShowDebug = true;

            Day02.StartA();
            Day02.StartB();

            Console.ReadKey();
        }
    }
}