using System;
using AdventOfCode2015.Days;

namespace AdventOfCode2015
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;

            Day1.StartA();
            Day1.StartB();

            Day2.StartA();
            Day2.StartB();

            Logger.ShowDebug = true;

            Day3.StartA();
            Day3.StartB();

            Console.ReadKey();
        }
    }
}
