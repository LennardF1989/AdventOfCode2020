using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day08
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day08_Test.txt")
                //.ReadAllLines("Content\\Day08_Test1.txt")
                .ReadAllLines("Content\\Day08.txt")
                .Select(x =>
                {
                    return x.Split("|")[1].Split(" ").Select(y => y.Trim()).ToList();
                })
                .ToList();

            int total = 0;

            foreach (var line in lines)
            {
                total += line.Count(x => x.Length == 2 || x.Length == 4 || x.Length == 3 || x.Length == 7);
            }

            Logger.Info($"Answer 8A: {total}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day08_Test.txt")
                //.ReadAllLines("Content\\Day08_Test1.txt")
                .ReadAllLines("Content\\Day08.txt")
                .Select(x =>
                {
                    var split = x.Split("|");

                    var leftRight = split.Select(y =>
                    {
                        return y
                            .Trim()
                            .Split(" ")
                            .Select(z => z.Trim())
                            .ToList();
                    }).ToList();

                    return new
                    {
                        left = leftRight[0],
                        right = leftRight[1]
                    };
                })
                .ToList();

            int total = 0;

            foreach (var line in lines)
            {
                //Get 1, 4, 7 and 8
                var d1 = line.left.Single(x => x.Length == 2);
                var d4 = line.left.Single(x => x.Length == 4);
                var d7 = line.left.Single(x => x.Length == 3);
                var d8 = line.left.Single(x => x.Length == 7);

                //Get 2, 3 and 5
                List<string> l5 = line.left.Where(x => x.Length == 5).ToList();

                //Get 0, 6 and 9
                List<string> l6 = line.left.Where(x => x.Length == 6).ToList();

                //3 contains all of 1
                var d3 = l5.Single(x => d1.All(x.Contains));
                l5.Remove(d3);

                //9 contains all of 3
                var d9 = l6.Single(x => d3.All(x.Contains));
                l6.Remove(d9);

                //0 contains all of 1
                var d0 = l6.Single(x => d1.All(x.Contains));
                l6.Remove(d0);
                var d6 = l6.Single();

                //6 contains all of 5
                var d5 = l5.Single(x => x.All(d6.Contains));
                l5.Remove(d5);
                var d2 = l5.Single();

                //Order the numbers alphabetically
                List<string> numbers = new List<string>
                {
                    string.Join(string.Empty, d0.OrderBy(x => x)),
                    string.Join(string.Empty, d1.OrderBy(x => x)),
                    string.Join(string.Empty, d2.OrderBy(x => x)),
                    string.Join(string.Empty, d3.OrderBy(x => x)),
                    string.Join(string.Empty, d4.OrderBy(x => x)),
                    string.Join(string.Empty, d5.OrderBy(x => x)),
                    string.Join(string.Empty, d6.OrderBy(x => x)),
                    string.Join(string.Empty, d7.OrderBy(x => x)),
                    string.Join(string.Empty, d8.OrderBy(x => x)),
                    string.Join(string.Empty, d9.OrderBy(x => x))
                };

                string output = string.Empty;
                foreach (var number in line.right)
                {
                    var n = string.Join("", number.OrderBy(x => x));

                    output += numbers.IndexOf(n);
                }

                total += int.Parse(output);
            }

            Logger.Info($"Answer 8B: {total}");
        }
    }
}
