using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day05
    {
        public static void StartA()
        {
            var lines = File
                    //.ReadAllText("Content\\Day05_Test.txt")
                    .ReadAllText("Content\\Day05.txt")
                ;

            var (stacks, moves) = ParseInput(lines);

            foreach (var move in moves)
            {
                for (int i = 0; i < move.amount; i++)
                {
                    var s = stacks[move.from - 1].Pop();
                    stacks[move.to - 1].Push(s);
                }
            }

            var answer = new StringBuilder();
            foreach (var stack in stacks)
            {
                answer.Append(stack.Peek());
            }

            Logger.Info($"Day 5A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllText("Content\\Day05_Test.txt")
                .ReadAllText("Content\\Day05.txt")
                ;

            var (stacks, moves) = ParseInput(lines);

            foreach (var move in moves)
            {
                List<char> popped = new List<char>();

                for (int i = 0; i < move.amount; i++)
                {
                    var s = stacks[move.from - 1].Pop();
                    popped.Add(s);
                }

                popped.Reverse();

                foreach (var s in popped)
                {
                    stacks[move.to - 1].Push(s);
                }
            }

            var answer = new StringBuilder();
            foreach (var stack in stacks)
            {
                answer.Append(stack.Peek());
            }

            Logger.Info($"Day 5B: {answer}");
        }

        private static (
            List<Stack<char>> stacks, 
            List<(int amount, int from, int to)> moves
        ) ParseInput(string text)
        {
            var lines = text.Split("\r\n\r\n");

            var stacksText = lines[0]
                .Split("\r\n")
                .Reverse()
                .Skip(1)
                .ToList();

            var stacks = new List<Stack<char>>();

            for (var index = 0; index < stacksText.Count; index++)
            {
                var stackText = stacksText[index];

                int currentStack = 0;
                for (int i = 0; i < stackText.Length;)
                {
                    if (index == 0)
                    {
                        stacks.Add(new Stack<char>());
                    }

                    var stack = stacks[currentStack++];

                    var crate = stackText
                        .Substring(i, 3)
                        .Trim();

                    if (crate.Length > 0)
                    {
                        stack.Push(crate[1]);
                    }

                    i += 4;
                }
            }

            var movesText = lines[1].Split("\r\n");
            var moves = new List<(int amount, int from, int to)>();

            foreach (var move in movesText)
            {
                var result = move.Split(" ");

                int stackNumber = int.Parse(result[1]);
                int stackFrom = int.Parse(result[3]);
                int stackTo = int.Parse(result[5]);

                moves.Add((stackNumber, stackFrom, stackTo));
            }

            return (stacks, moves);
        }
    }
}
