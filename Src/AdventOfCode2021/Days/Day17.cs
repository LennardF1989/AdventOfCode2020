using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day17
    {
        public static void Start()
        {
            var lines = File
                //.ReadAllText("Content\\Day17_Test.txt")
                .ReadAllText("Content\\Day17.txt")
                .Split(":")[1]
                .Split(",")
                .ToList();

            var minMaxX = GetMinMax(lines[0].Split("=")[1]);
            var minMaxY = GetMinMax(lines[1].Split("=")[1]);
            var bounds = (minMaxX, minMaxY);

            var highestY = 0;
            var hits = 0;
            var hitVelocities = new HashSet<(int, int)>();

            //NOTE: There is probably a way to calculate the absolute minimum and maximum beforehand
            var maxY = Math.Abs(minMaxY.max) * 2;
            var maxX = minMaxX.max * 2;

            for (int y = minMaxY.min; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    var result = TraceTrajectory(x, y, bounds);

                    if (result.hit)
                    {
                        hits++;

                        hitVelocities.Add((x, y));

                        if (result.highestY > highestY)
                        {
                            highestY = result.Item2;
                        }
                    }
                }
            }

            Logger.Debug($"Number of Hits: {hits}");

            int answer = highestY;

            Logger.Info($"Day 17A: {answer}");
            Logger.Info($"Day 17B: {hitVelocities.Count}");
        }

        private static (bool hit, int highestY) TraceTrajectory(int velocityX, int velocityY, ((int min, int max) x, (int min, int max) y) bounds)
        {
            int x = 0;
            int y = 0;

            int highestY = 0;

            while (true)
            {
                x += velocityX;
                y += velocityY;

                if (y > highestY)
                {
                    highestY = y;
                }

                if (x >= bounds.x.min && x <= bounds.x.max && y >= bounds.y.min && y <= bounds.y.max)
                {
                    //Hit
                    return (true, highestY);
                }
                
                if (x > bounds.x.max || y < bounds.y.min)
                {
                    //No hit
                    return (false, highestY);
                }

                if (velocityX > 0)
                {
                    velocityX -= 1;
                }
                else if (velocityX < 0)
                {
                    velocityX += 1;
                }

                velocityY -= 1;
            }
        }

        private static (int min, int max) GetMinMax(string value)
        {
            var minMax = value
                .Split("..")
                .Select(y => int.Parse(y.Trim()))
                .ToList();

            return (min: minMax[0], max: minMax[1]);
        }
    }
}
