using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2015.Days
{
    public static class Day02
    {
        public static void StartA()
        {
            var lines = File.ReadAllLines("Content\\Day02.txt");

            int answer = 0;

            foreach (string line in lines)
            {
                var splittedLines = line.Split("x");

                int l = int.Parse(splittedLines[0]);
                int w = int.Parse(splittedLines[1]);
                int h = int.Parse(splittedLines[2]);

                int wl = w * l;
                int wh = w * h;
                int hl = h * l;

                int surfaceArea = 2 * wl + 2 * wh + 2 * hl;
                int extraArea = Math.Min(wl, Math.Min(wh, hl));

                int totalArea = surfaceArea + extraArea;

                Logger.Debug($"{totalArea}");

                answer += totalArea;
            }

            Logger.Info($"Day 2A: {answer}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day02_Test.txt");
            var lines = File.ReadAllLines("Content\\Day02.txt");

            int answer = 0;

            foreach (string line in lines)
            {
                var splittedLines = line.Split("x");

                int l = int.Parse(splittedLines[0]);
                int w = int.Parse(splittedLines[1]);
                int h = int.Parse(splittedLines[2]);

                List<int> sides = new List<int> {l, w, h};
                sides.Remove(sides.Max());

                int ribbon = sides[0] + sides[0] + sides[1] + sides[1];
                int extraRibbon = l * w * h;

                int totalRibbon = ribbon + extraRibbon;

                Logger.Debug($"{totalRibbon}");

                answer += totalRibbon;
            }

            Logger.Info($"Day 2B: {answer}");
        }
    }
}
