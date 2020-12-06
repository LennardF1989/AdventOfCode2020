using System;
using AdventOfCode.Days;

namespace AdventOfCode
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

            Logger.ShowDebug = true;

            Day7.StartA();
            Day7.StartB();

            Console.ReadKey();
        }
    }
}
