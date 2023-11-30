using System;
using AdventOfCode.Shared;
using AdventOfCode2023.Days;

namespace AdventOfCode2023
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;
            
            //if(false)
            {
                Leaderboard.Start();
            }

            Logger.ShowDebug = true;

            Day01.StartA();
            Day01.StartB();

            Console.ReadKey();
        }
    }
}