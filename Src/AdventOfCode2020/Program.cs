﻿using System;
using AdventOfCode2020.Days;

namespace AdventOfCode2020
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

            Logger.ShowDebug = true;

            Day10.StartA();
            Day10.StartB();

            Console.ReadKey();
        }
    }
}
