using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day25
    {
        private enum Seacumcumber
        {
            Empty = 0,
            Right,
            Down
        }

        public static void Start()
        {
            var lines = File
                //.ReadAllLines("Content\\Day25_Test.txt")
                .ReadAllLines("Content\\Day25.txt")
                .Select(x => x.Select(y =>
                {
                    return y switch
                    {
                        '>' => Seacumcumber.Right,
                        'v' => Seacumcumber.Down,
                        _ => Seacumcumber.Empty
                    };
                }).ToList())
                .ToList();

            Logger.Debug("Initial state:");
            var currentGrid = PrintGrid(lines);
            var previousGrid = currentGrid;

            int steps = 0;

            while (true)
            {
                steps++;

                Logger.Debug($"After {steps} step:");

                lines = RunStep(lines);
                currentGrid = PrintGrid(lines);

                if (currentGrid == previousGrid)
                {
                    break;
                }

                previousGrid = currentGrid;
            }

            int answer = steps;

            Logger.Info($"Day 25A: {answer}");
        }

        private static List<List<Seacumcumber>> RunStep(List<List<Seacumcumber>> lines)
        {
            //Make copy
            var linesCopy = new List<List<Seacumcumber>>();

            foreach (var y in lines)
            {
                linesCopy.Add(new List<Seacumcumber>(y.ToList()));
            }

            //Move right
            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Count; x++)
                {
                    var result = lines[y][x];

                    if (result == Seacumcumber.Right)
                    {
                        var correctedX = (x + 1) % lines[y].Count;

                        var neighbour = lines[y][correctedX];

                        if (neighbour == Seacumcumber.Empty)
                        {
                            linesCopy[y][x] = Seacumcumber.Empty;
                            linesCopy[y][correctedX] = result;
                        }
                    }
                }
            }

            //Swap copy
            lines = linesCopy;

            //Make copy
            linesCopy = new List<List<Seacumcumber>>();

            foreach (var y in lines)
            {
                linesCopy.Add(new List<Seacumcumber>(y.ToList()));
            }

            //Move down
            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Count; x++)
                {
                    var result = lines[y][x];

                    if (result == Seacumcumber.Down)
                    {
                        var correctedY = (y + 1) % lines.Count;

                        var neighbour = lines[correctedY][x];

                        if (neighbour == Seacumcumber.Empty)
                        {
                            linesCopy[y][x] = Seacumcumber.Empty;
                            linesCopy[correctedY][x] = result;
                        }
                    }
                }
            }

            return linesCopy;
        }

        private static string PrintGrid(List<List<Seacumcumber>> lines)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Count; x++)
                {
                    var value = lines[y][x];

                    switch (value)
                    {
                        case Seacumcumber.Right:
                            stringBuilder.Append('>');
                            break;
                        case Seacumcumber.Down:
                            stringBuilder.Append('v');
                            break;
                        default:
                            stringBuilder.Append('.');
                            break;
                    }
                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());

            return stringBuilder.ToString();
        }
    }
}
