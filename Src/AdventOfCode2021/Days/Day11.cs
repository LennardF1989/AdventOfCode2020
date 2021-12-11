using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day11
    {
        private static readonly List<(int x, int y)> _surroundingCoords = new()
        {
            (-1, -1),
            (0, -1),
            (1, -1),
            (-1, 0),
            (1, 0),
            (-1, 1),
            (0, 1),
            (1, 1)
        };

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day11_Test.txt")
                .ReadAllLines("Content\\Day11.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList();

            int answer = 0;

            //LogLines(lines);

            for (int i = 0; i < 100; i++)
            {
                answer += RunStep(lines);

                //LogLines(lines);
            }

            Logger.Info($"Day 11A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day11_Test.txt")
                .ReadAllLines("Content\\Day11.txt")
                .Select(x => x.Select(y => int.Parse(y.ToString())).ToList())
                .ToList();

            int answer = 0;

            //LogLines(lines);

            for (int i = 0; i < 500; i++)
            {
                RunStep(lines);

                //LogLines(lines);

                if (lines.All(y => y.All(x => x == 0)))
                {
                    answer = i + 1;

                    break;
                }
            }

            Logger.Info($"Day 11B: {answer}");
        }

        private static int RunStep(List<List<int>> lines)
        {
            Queue<(int x, int y)> flashingOctopus = new Queue<(int x, int y)>();

            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Count; x++)
                {
                    int newValue = IncreaseValue(lines, x, y);

                    if (newValue == -1)
                    {
                        continue;
                    }

                    if (newValue == 9)
                    {
                        flashingOctopus.Enqueue((x, y));
                    }
                }
            }
            
            List<(int x, int y)> resetOctopus = new List<(int x, int y)>();

            while (flashingOctopus.Count > 0)
            {
                var tuple = flashingOctopus.Dequeue();

                _surroundingCoords.ForEach(tuple2 =>
                {
                    int newValue = IncreaseValue(lines, tuple.x + tuple2.x, tuple.y + tuple2.y);

                    if (newValue == -1)
                    {
                        return;
                    }

                    if (newValue == 9)
                    {
                        flashingOctopus.Enqueue((tuple.x + tuple2.x, tuple.y + tuple2.y));
                    }
                });

                resetOctopus.Add(tuple);
            }

            int flashes = resetOctopus.Count;

            resetOctopus.ForEach(x => ResetValue(lines, x.x, x.y));

            return flashes;
        }

        private static int GetValue(List<List<int>> lines, int x, int y)
        {
            if (y < 0 || y >= lines.Count || x < 0 || x >= lines[y].Count)
            {
                return -1;
            }

            return lines[y][x];
        }

        private static int IncreaseValue(List<List<int>> lines, int x, int y)
        {
            if (y < 0 || y >= lines.Count || x < 0 || x >= lines[y].Count)
            {
                return -1;
            }

            int newValue = lines[y][x]++;

            return newValue;
        }

        private static void ResetValue(List<List<int>> lines, int x, int y)
        {
            lines[y][x] = 0;
        }

        private static void LogLines(List<List<int>> lines)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Count; x++)
                {
                    int value = GetValue(lines, x, y);

                    stringBuilder.Append(value);
                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());
        }
    }
}
