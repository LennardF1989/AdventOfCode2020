using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day15
    {
        private class IndexedStack
        {
            private int _last;
            private int _other;

            public IndexedStack(int first)
            {
                Push(first);
            }

            public void Push(int value)
            {
                _other = _last;
                _last = value;
            }

            public int Difference()
            {
                return _last - _other;
            }

            public override string ToString()
            {
                return $"[{_last}, {_other}]";
            }
        }

        private struct IndexedStack2
        {
            private int _last;
            private int _other;

            public IndexedStack2(int first)
            {
                _last = first;
                _other = 0;
            }

            public void Push(int value)
            {
                _other = _last;
                _last = value;
            }

            public int Difference()
            {
                return _last - _other;
            }

            public override string ToString()
            {
                return $"[{_last}, {_other}]";
            }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day15_Test.txt");
            var lines = File.ReadAllLines("Content\\Day15.txt");

            foreach (var line in lines)
            {
                var result = DetermineNumber(line.Split(",").Select(int.Parse).ToList(), 2020);

                Logger.Info($"Day 15A: {result}");
            }
        }

        private static int DetermineNumber(List<int> startingNumbers, int targetTurn)
        {
            var spokenNumbers = new Dictionary<int, IndexedStack>(targetTurn);

            for (int i = 0; i < startingNumbers.Count - 1; i++)
            {
                //Logger.Debug($"Turn {i + 1} = {startingNumbers[i]}");

                spokenNumbers.Add(startingNumbers[i], new IndexedStack(i + 1));
            }

            int lastNumber = startingNumbers[^1];

            for (int i = startingNumbers.Count - 1; i < targetTurn - 1; i++)
            {
                //Logger.Debug($"Turn {i + 1} = {lastNumber}");

                if (spokenNumbers.TryGetValue(lastNumber, out var result))
                {
                    result.Push(i + 1);

                    lastNumber = result.Difference();
                }
                else
                {
                    spokenNumbers.Add(lastNumber, new IndexedStack(i + 1));

                    lastNumber = 0;
                }
            }

            //Logger.Debug($"Turn {targetTurn} = {lastNumber}");

            return lastNumber;
        }

        private static int DetermineNumberOptimized(List<int> startingNumbers, int targetTurn)
        {
            var spokenNumbers = new List<IndexedStack2>(targetTurn);
            spokenNumbers.AddRange(Enumerable.Repeat(new IndexedStack2(0), targetTurn));

            for (int i = 0; i < startingNumbers.Count - 1; i++)
            {
                var spokenNumber = spokenNumbers[i];
                spokenNumber.Push(i + 1);
                spokenNumbers[startingNumbers[i]] = spokenNumber;
            }

            int lastNumber = startingNumbers[^1];

            for (int i = startingNumbers.Count - 1; i < targetTurn - 1; i++)
            {
                var spokenNumber = spokenNumbers[lastNumber];
                var difference = spokenNumber.Difference();
                spokenNumber.Push(i + 1);
                spokenNumbers[lastNumber] = spokenNumber;
                lastNumber = difference == 0 ? 0 : spokenNumber.Difference();
            }

            return lastNumber;
        }

        private static int DetermineNumberSuperOptimized(int[] startingNumbers, int targetTurn)
        {
            var spokenNumbers = new int[targetTurn];

            for (int i = 0; i < startingNumbers.Length - 1; i++)
            {
                spokenNumbers[startingNumbers[i]] = i + 1;
            }

            int lastNumber = startingNumbers[^1];

            for (int i = startingNumbers.Length - 1; i < targetTurn - 1; i++)
            {
                var spokenNumber = spokenNumbers[lastNumber];
                spokenNumbers[lastNumber] = i + 1;
                lastNumber = spokenNumber == 0 ? 0 : i + 1 - spokenNumber;
            }

            return lastNumber;
        }

        private static void Benchmark(int[] startingNumbers)
        {
            Stopwatch stopwatch = new Stopwatch();

            var timings = new long[100];

            for (int i = 0; i < 100; i++)
            {
                stopwatch.Start();
                DetermineNumberSuperOptimized(startingNumbers, 30_000_000);
                stopwatch.Stop();
                timings[i] = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }
                
            Logger.Debug($"Timing: {timings.Average()}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day15_Test.txt");
            var lines = File.ReadAllLines("Content\\Day15.txt");

            foreach (var line in lines)
            {
                var startingNumbers = line
                    .Split(",")
                    .Select(int.Parse)
                    .ToList();

                //var result = DetermineNumber(startingNumbers, 30_000_000);
                //var result = DetermineNumberOptimized(startingNumbers, 30_000_000);
                var result = DetermineNumberSuperOptimized(startingNumbers.ToArray(), 30_000_000);

                //Benchmark(startingNumbers.ToArray());

                Logger.Info($"Day 15B: {result}");
            }
        }
    }
}
