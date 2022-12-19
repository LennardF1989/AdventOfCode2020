using System;
using AdventOfCode.Shared;
using AdventOfCode2022.Days;

namespace AdventOfCode2022
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logger.ShowDebug = false;
            
            //if(false)
            {
                Leaderboard.Start();

                Day01.Start();
                Day02.StartA();
                Day02.StartB();
                Day03.StartA();
                Day03.StartB();
                Day04.StartA();
                Day04.StartB();
                Day05.StartA();
                Day05.StartB();
                Day06.StartA();
                Day06.StartB();
                Day07.StartA();
                Day07.StartB();
                Day08.StartA();
                Day08.StartB();
                Day09.StartA();
                Day09.StartB();
                Day10.StartA();
                Day10.StartB();
                Day11.StartA();
                Day11.StartB();
                Day12.StartA();
                Day12.StartB();
                Day13.StartA();
                Day13.StartB();
                Day14.StartA();
                Day14.StartB();
                Day15.StartA();
                Day15.StartB();
                Day16.StartA();
                Day16.StartB();
                Day17.StartA();
                Day17.StartB();
                Day18.StartA();
                Day18.StartB();
                Day19.StartA();
                Day19.StartB();
            }

            Logger.ShowDebug = true;

            Day20.StartA();
            Day20.StartB();

            Console.ReadKey();
        }
    }
}