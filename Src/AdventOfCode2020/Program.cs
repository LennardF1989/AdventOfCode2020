using System;
using AdventOfCode2020.Days;

namespace AdventOfCode2020
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

            Day3.StartA();
            Day3.StartB();

            Day4.StartA();
            Day4.StartB();

            Day5.StartA();
            Day5.StartB();

            Day6.StartA();
            Day6.StartB();

            Day7.StartA();
            Day7.StartB();

            Day8.StartA();
            Day8.StartB();

            Logger.ShowDebug = true;

            Day9.StartA();
            Day9.StartB();

            Console.ReadKey();
        }
    }
}
