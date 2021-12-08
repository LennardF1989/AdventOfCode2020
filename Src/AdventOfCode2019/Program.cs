﻿using System;
using AdventOfCode.Shared;
using AdventOfCode2019.Days;

namespace AdventOfCode2019
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

            Logger.ShowDebug = true;

            Day05.StartA();
            Day05.StartB();

            Console.ReadKey();
        }
    }
}
