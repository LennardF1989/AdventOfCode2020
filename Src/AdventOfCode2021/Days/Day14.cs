using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day14
    {
        public static void StartA()
        {
            var lines = File
                .ReadAllText("Content\\Day14_Test.txt")
                //.ReadAllText("Content\\Day14.txt")
                .Split("\r\n\r\n")
                ;

            var polymerTemplate = lines[0];

            var rules = lines[1]
                .Split("\r\n")
                .Select(x => x.Split(" -> "))
                .ToDictionary(x => (x[0][0], x[0][1]), x => x[1]);

            int answer = RunStep(polymerTemplate, rules, 10);

            Logger.Info($"Day 14A: {answer}");
        }

        private static int RunStep(string polymerTemplate, Dictionary<(char, char), string> rules, int maxStep)
        {
            var list = polymerTemplate.ToList();

            //Logger.Debug($"Template: {string.Join("", list)}");

            var listCopy = RunStep(list, rules, 1, maxStep);

            var chars = listCopy
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var min = chars.Min(x => x.Value);
            var max = chars.Max(x => x.Value);

            return max - min;
        }

        private static List<char> RunStep(List<char> list, Dictionary<(char, char), string> rules, int step, int maxStep)
        {
            var listCopy = new List<char>(list.Count * 2);

            listCopy.Add(list[0]);

            for (int i = 0; i < list.Count - 1; i++)
            {
                char a = list[i];
                char b = list[i + 1];

                if (!rules.TryGetValue((a, b), out var rule))
                {
                    continue;
                }

                listCopy.Add(rule[0]);
                listCopy.Add(b);
            }

            //Logger.Debug($"After step {step}: {string.Join("", listCopy)}");

            if (step < maxStep)
            {
                return RunStep(listCopy, rules, step + 1, maxStep);
            }

            return listCopy;
        }

        public static void StartB()
        {
            var lines = File
                    .ReadAllText("Content\\Day14_Test.txt")
                    //.ReadAllText("Content\\Day14.txt")
                    .Split("\r\n\r\n")
                ;

            var polymerTemplate = lines[0];

            var rules = lines[1]
                .Split("\r\n")
                .Select(x => x.Split(" -> "))
                .ToDictionary(x => (x[0][0], x[0][1]), x => x[1]);

            //long answer = RunStep2(polymerTemplate, rules, 40);
            long answer = RunStep3(polymerTemplate, rules, 40);

            Logger.Info($"Day 14B: {answer}");
        }

        //Disclaimer: I got a lot of hints for this solution
        private static long RunStep2(string polymerTemplate, Dictionary<(char, char), string> rules, int maxStep)
        {
            var pairs = new Dictionary<(char, char), long>();

            for (int i = 0; i < polymerTemplate.Length - 1; i++)
            {
                var pair = (polymerTemplate[i], polymerTemplate[i + 1]);

                if (pairs.ContainsKey(pair))
                {
                    pairs[pair]++;
                }
                else
                {
                    pairs.Add(pair, 1);
                }
            }

            for (int i = 0; i < maxStep; i++)
            {
                var tempPairs = new Dictionary<(char, char), long>();

                while (pairs.Count > 0)
                {
                    var kvp = pairs.First();

                    var a = kvp.Key.Item1;
                    var b = kvp.Key.Item2;

                    pairs.Remove(kvp.Key);

                    if (rules.ContainsKey(kvp.Key))
                    {
                        var c = rules[kvp.Key][0];

                        if (tempPairs.ContainsKey((a, c)))
                        {
                            tempPairs[(a, c)] += kvp.Value;
                        }
                        else
                        {
                            tempPairs.Add((a, c), kvp.Value);
                        }

                        if (tempPairs.ContainsKey((c, b)))
                        {
                            tempPairs[(c, b)] += kvp.Value;
                        }
                        else
                        {
                            tempPairs.Add((c, b), kvp.Value);
                        }
                    }
                    else
                    {
                        tempPairs[kvp.Key] = kvp.Value;
                    }
                }

                pairs = tempPairs;
            }

            var chars = new Dictionary<char, double>();

            pairs
                .SelectMany(x =>
                {
                    return new[]
                    {
                        (x.Key.Item1, x.Value),
                        (x.Key.Item2, x.Value)
                    };
                })
                .ToList()
                .ForEach(x =>
                {
                    if (chars.ContainsKey(x.Item1))
                    {
                        chars[x.Item1] += x.Value / 2.0;
                    }
                    else
                    {
                        chars.Add(x.Item1, x.Value / 2.0);
                    }
                });

            var min = chars.Min(x => x.Value);
            var max = chars.Max(x => x.Value);

            //NOTE: This is technically incorrect, but works for my input.
            return (long)Math.Floor(max - min);
        }

        //Disclaimer: A little bit of mine and a little bit of Reddit
        private static long RunStep3(string polymerTemplate, Dictionary<(char, char), string> rules, int maxStep)
        {
            var pairs = new Dictionary<(char, char), long>();

            for (int i = 0; i < polymerTemplate.Length - 1; i++)
            {
                var pair = (polymerTemplate[i], polymerTemplate[i + 1]);

                if (pairs.ContainsKey(pair))
                {
                    pairs[pair]++;
                }
                else
                {
                    pairs.Add(pair, 1);
                }
            }

            var chars = polymerTemplate
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => (long)x.Count());

            for (int i = 0; i < maxStep; i++)
            {
                var tempPairs = new Dictionary<(char, char), long>();

                foreach (var pair in pairs)
                {
                    if (!rules.ContainsKey(pair.Key))
                    {
                        continue;
                    }

                    char a = pair.Key.Item1;
                    char c = rules[pair.Key][0];
                    char b = pair.Key.Item2;

                    if (tempPairs.ContainsKey((a, c)))
                    {
                        tempPairs[(a, c)] += pair.Value;
                    }
                    else
                    {
                        tempPairs.Add((a, c), pair.Value);
                    }

                    if (tempPairs.ContainsKey((c, b)))
                    {
                        tempPairs[(c, b)] += pair.Value;
                    }
                    else
                    {
                        tempPairs.Add((c, b), pair.Value);
                    }

                    if(chars.ContainsKey(c))
                    {
                        chars[c] += pair.Value;
                    }
                    else
                    {
                        chars[c] = pair.Value;
                    }
                }

                pairs = tempPairs;
            }

            var min = chars.Min(x => x.Value);
            var max = chars.Max(x => x.Value);

            return max - min;
        }
    }
}
