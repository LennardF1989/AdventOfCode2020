using System;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day04
    {
        //NOTE: Original version
        static class Solution1
        {
            public static void StartA()
            {
                var answer = File
                        //.ReadAllLines("Content\\Day04_Test.txt")
                        .ReadAllLines("Content\\Day04.txt")
                        .Select(GetPairs)
                        .Count(pairs =>
                            (pairs.minA <= pairs.minB && pairs.maxA >= pairs.maxB) ||
                            (pairs.minB <= pairs.minA && pairs.maxB >= pairs.maxA)
                        )
                    ;

                Logger.Info($"Day 4A: {answer}");
            }

            public static void StartB()
            {
                var answer = File
                    //.ReadAllLines("Content\\Day04_Test.txt")
                    .ReadAllLines("Content\\Day04.txt")
                    .Select(GetPairs)
                    .Count(pairs =>
                        Math.Max(pairs.minA, pairs.minB) <= Math.Min(pairs.maxA, pairs.maxB)
                    );

                Logger.Info($"Day 4B: {answer}");
            }

            private static (int minA, int maxA, int minB, int maxB) GetPairs(string pair)
            {
                var pairs = pair.Split(",");

                var pairA = pairs[0].Split("-");
                var minA = int.Parse(pairA[0]);
                var maxA = int.Parse(pairA[1]);

                var pairB = pairs[1].Split("-");
                var minB = int.Parse(pairB[0]);
                var maxB = int.Parse(pairB[1]);

                return (minA, maxA, minB, maxB);
            }
        }

        //NOTE: Alternative version
        static class Solution2
        {
            readonly struct BoundingBox1D
            {
                public int Left { get; }
                public int Right { get; }

                public BoundingBox1D(int left, int right)
                {
                    Left = left;
                    Right = right;
                }

                public bool Contains(BoundingBox1D other)
                {
                    return Left <= other.Left && Right >= other.Right;
                }

                public bool Overlaps(BoundingBox1D other)
                {
                    return Left <= other.Right && other.Left <= Right;
                }
            }

            public static void StartA()
            {
                var answer = File
                    //.ReadAllLines("Content\\Day04_Test.txt")
                    .ReadAllLines("Content\\Day04.txt")
                    .Select(GetPairs)
                    .Count(pairs =>
                        pairs.A.Contains(pairs.B) ||
                        pairs.B.Contains(pairs.A)
                    );

                Logger.Info($"Day 4A: {answer}");
            }

            public static void StartB()
            {
                var answer = File
                    //.ReadAllLines("Content\\Day04_Test.txt")
                    .ReadAllLines("Content\\Day04.txt")
                    .Select(GetPairs)
                    .Count(pairs =>
                        pairs.A.Overlaps(pairs.B)
                    );

                Logger.Info($"Day 4B: {answer}");
            }

            private static (BoundingBox1D A, BoundingBox1D B) GetPairs(string pair)
            {
                var pairs = pair.Split(",");

                var pairA = pairs[0].Split("-");
                var minA = int.Parse(pairA[0]);
                var maxA = int.Parse(pairA[1]);

                var pairB = pairs[1].Split("-");
                var minB = int.Parse(pairB[0]);
                var maxB = int.Parse(pairB[1]);

                return (
                    new BoundingBox1D(minA, maxA),
                    new BoundingBox1D(minB, maxB)
                );
            }
        }

        public static void StartA()
        {
            //Solution1.StartA();
            Solution2.StartA();
        }

        public static void StartB()
        {
            //Solution1.StartB();
            Solution2.StartB();
        }
    }
}
