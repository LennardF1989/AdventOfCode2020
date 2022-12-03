using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day03
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt")
                .ReadAllLines("Content\\Day03.txt")
                ;
            
            var answer = 0;

            foreach (var line in lines)
            {
                var part1 = line.Substring(0, line.Length / 2);
                var part2 = line.Substring(line.Length / 2);

                var part1List = new HashSet<char>(part1.ToCharArray());
                var part2List = new HashSet<char>(part2.ToCharArray());

                foreach (var p in part1List)
                {
                    if (!part2List.Contains(p))
                    {
                        continue;
                    }

                    answer += GetScore(p);

                    break;
                }
            }

            Logger.Info($"Day 3A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt")
                .ReadAllLines("Content\\Day03.txt")
                ;

            var answer = 0;

            for (var i = 0; i < lines.Length / 3; i++)
            {
                var line1 = lines[(i*3)];
                var line2 = lines[(i * 3) + 1];
                var line3 = lines[(i * 3) + 2];

                var mapping = new Dictionary<char, int>();

                UpdateMapping(mapping, new HashSet<char>(line1.ToCharArray()));
                UpdateMapping(mapping, new HashSet<char>(line2.ToCharArray()));
                UpdateMapping(mapping, new HashSet<char>(line3.ToCharArray()));

                var p = mapping.First(x => x.Value == 3).Key;

                answer += GetScore(p);
            }

            Logger.Info($"Day 3B: {answer}");
        }

        private static void UpdateMapping(Dictionary<char, int> mapping, HashSet<char> hashSet)
        {
            foreach (var c in hashSet)
            {
                if (!mapping.ContainsKey(c))
                {
                    mapping[c] = 0;
                }

                mapping[c]++;
            }
        }

        private static int GetScore(char c)
        {
            return c >= 'a' 
                ? c - 'a' + 1 
                : (c - 'A') + 1 + 26;
        }
    }
}
