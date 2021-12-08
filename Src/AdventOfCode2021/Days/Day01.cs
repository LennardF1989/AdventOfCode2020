using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day01
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                .Select(int.Parse)
                .ToList();

            int previous = lines[0];
            int increased = 0;

            for (var i = 1; i < lines.Count; i++)
            {
                if (lines[i] > previous)
                {
                    increased++;
                }

                previous = lines[i];
            }

            Logger.Info($"Answer 1A: {increased}");
        }

        public static void StartB()
        {
            var lines1 = File
                //.ReadAllLines("Content\\Day01_Test.txt")
                .ReadAllLines("Content\\Day01.txt")
                .Select(int.Parse)
                .ToList();

            List<int> depths = new List<int>();

            for(int i = 0; i < lines1.Count - 2; i++)
            {
                int depth = lines1[i] + lines1[i + 1] + lines1[i + 2];

                depths.Add(depth);
            } 

            int previous = depths[0];
            int increased = 0;

            for (var i = 1; i < depths.Count; i++)
            {
                if (depths[i] > previous)
                {
                    increased++;
                }

                previous = depths[i];
            }

            Logger.Info($"Answer 1B: {increased}");
        }
    }
}
