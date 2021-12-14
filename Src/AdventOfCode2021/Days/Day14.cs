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

            int answer = RunStep2(polymerTemplate, rules, 40);

            Logger.Info($"Day 14B: {answer}");
        }

        private static int RunStep2(string polymerTemplate, Dictionary<(char, char), string> rules, int maxStep)
        {
            var list = polymerTemplate.ToList();

            //Logger.Debug($"Template: {string.Join("", list)}");

            var chars = new Dictionary<char, int>();

            list = RunStep(list, rules, 1, 10);

            for (int i = 0; i < (list.Count / 2) + 1; i++)
            {
                var toProcess = list
                    .Skip(i)
                    .Take(2)
                    .ToList();

                var listCopy = RunStep(toProcess, rules, 11, maxStep);

                var charsCopy = listCopy
                    .GroupBy(x => x)
                    .ToList();

                charsCopy.ForEach(x =>
                {
                    if (chars.ContainsKey(x.Key))
                    {
                        chars[x.Key] += x.Count();
                    }
                    else
                    {
                        chars.Add(x.Key, x.Count());
                    }
                });
            }

            var min = chars.Min(x => x.Value);
            var max = chars.Max(x => x.Value);

            return max - min;
        }
    }
}
