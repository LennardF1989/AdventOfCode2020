using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day10
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day10_Test.txt")
                .ReadAllLines("Content\\Day10.txt")
                ;

            int points = lines.Sum(x =>
            {
                var stack = new Stack<char>();

                return CheckLine(x, stack);
            });

            Logger.Info($"Day 10A: {points}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day10_Test.txt")
                .ReadAllLines("Content\\Day10.txt")
                ;

            List<long> scores = new List<long>();

            foreach (var line in lines)
            {
                var stack = new Stack<char>();

                int points = CheckLine(line, stack);

                if (points != 0 || stack.Count <= 0)
                {
                    continue;
                }

                long localScore = 0;

                while (stack.Count > 0)
                {
                    var c = stack.Pop();

                    localScore = c switch
                    {
                        '(' => (localScore * 5) + 1,
                        '[' => (localScore * 5) + 2,
                        '{' => (localScore * 5) + 3,
                        '<' => (localScore * 5) + 4,
                        _ => localScore
                    };
                }

                scores.Add(localScore);
            }

            scores.Sort();

            long answer = scores[(int)Math.Floor(scores.Count / 2f)];

            Logger.Info($"Day 10B: {answer}");
        }

        private static int CheckLine(string line, Stack<char> stack)
        {
            foreach (var c in line)
            {
                switch (c)
                {
                    case '(' or '[' or '{' or '<':
                        stack.Push(c);
                        break;

                    case ')' when stack.Pop() != '(':
                        return 3;

                    case ']' when stack.Pop() != '[':
                        return 57;

                    case '}' when stack.Pop() != '{':
                        return 1197;

                    case '>' when stack.Pop() != '<':
                        return 25137;
                }
            }

            return 0;
        }
    }
}
