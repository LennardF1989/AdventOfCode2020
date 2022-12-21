using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day21
    {
        private sealed record Entry(string monkey, string type, int value, string variable1, string variable2, char op);

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day21_Test.txt")
                .ReadAllLines("Content\\Day21.txt")
                .Select(ParseInput)
                .ToList();

            var allMonkeys = lines
                .Where(x => x.type == "v")
                .ToDictionary(x => x.monkey, v => (long)v.value);

            var allOtherMonkeys = lines
                .Where(x => !allMonkeys.ContainsKey(x.monkey))
                .ToDictionary(x => x.monkey);

            SolveMonkeys(allMonkeys, allOtherMonkeys);

            var answer = allMonkeys["root"];

            Logger.Info($"Day 21A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day21_Test.txt")
                .ReadAllLines("Content\\Day21.txt")
                .Select(ParseInput)
                .ToList();

            var monkeys = lines.ToDictionary(x => x.monkey);
            var root = monkeys["root"];
            var human = monkeys["humn"];

            //Remove human
            lines.Remove(human);

            //Create a path from human to root so we can exclude those
            var pathFromHumanToRoot = new List<Entry>();
            var current = human;

            while (current != root)
            {
                current = monkeys.FirstOrDefault(
                    x => x.Value.type == "o" &&
                    (x.Value.variable1 == current.monkey || x.Value.variable2 == current.monkey)
                ).Value;

                pathFromHumanToRoot.Add(current);
            }

            //Solve the remaining monkeys
            var allMonkeys = lines
                .Where(x => x.type == "v")
                .ToDictionary(x => x.monkey, v => (long)v.value);

            var allOtherMonkeys = lines
                .Where(x => !pathFromHumanToRoot.Contains(x) && !allMonkeys.ContainsKey(x.monkey))
                .ToDictionary(x => x.monkey);

            SolveMonkeys(allMonkeys, allOtherMonkeys);

            //Set the unknown root variable to the known value
            var otherVariable = pathFromHumanToRoot[^2].monkey;
            var variableToCheck = root.variable1 == otherVariable ? root.variable2 : root.variable1;
            allMonkeys.Add(otherVariable, allMonkeys[variableToCheck]);

            /*void Validate()
            {
                var v1 = allMonkeys[current.variable1];
                var v2 = allMonkeys[current.variable2];

                var result = current.op switch
                {
                    '+' => v1 + v2,
                    '*' => v1 * v2,
                    '/' => v1 / v2,
                    '-' => v1 - v2
                };

                if (result != allMonkeys[current.monkey])
                {
                    //PANIC
                }
            }*/

            //Follow the path from root to human
            current = monkeys[otherVariable];

            while (current != human)
            {
                /*if (!allMonkeys.ContainsKey(current.variable1) && 
                    !allMonkeys.ContainsKey(current.variable2))
                {
                    //PANIC!
                }*/

                if (!allMonkeys.ContainsKey(current.variable1))
                {
                    var targetValue = allMonkeys[current.monkey];
                    var v2 = allMonkeys[current.variable2];

                    var result = current.op switch
                    {
                        '+' => targetValue - v2,
                        '*' => targetValue / v2,
                        '/' => targetValue * v2,
                        '-' => targetValue + v2
                    };

                    allMonkeys.Add(current.variable1, result);

                    //Validate();

                    current = monkeys[current.variable1];
                }
                else if (!allMonkeys.ContainsKey(current.variable2))
                {
                    var targetValue = allMonkeys[current.monkey];
                    var v1 = allMonkeys[current.variable1];

                    var result = current.op switch
                    {
                        '+' => targetValue - v1,
                        '*' => targetValue / v1,
                        '/' => v1 * targetValue,
                        '-' => v1 - targetValue
                    };
                    
                    allMonkeys.Add(current.variable2, result);

                    //Validate();

                    current = monkeys[current.variable2];
                }
            }

            var answer = allMonkeys["humn"];

            Logger.Info($"Day 21B: {answer}");
        }

        private static Entry ParseInput(string x)
        {
            var splitted = x.Split(":");

            string monkey = splitted[0];
            string variable1 = null;
            char op = ' ';
            string variable2 = null;
            int value = 0;
            string type;

            if (splitted[1].Trim().Contains(" "))
            {
                var splitted2 = splitted[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                type = "o";
                variable1 = splitted2[0];
                op = splitted2[1][0];
                variable2 = splitted2[2];
            }
            else
            {
                type = "v";
                value = int.Parse(splitted[1]);
            }

            return new Entry(monkey, type, value, variable1, variable2, op);
        }

        private static void SolveMonkeys(Dictionary<string, long> allMonkeys, Dictionary<string, Entry> allOtherMonkeys)
        {
            while (allOtherMonkeys.Count > 0)
            {
                var solvedMonkeys = allOtherMonkeys.Where(x =>
                    allMonkeys.ContainsKey(x.Value.variable1) && allMonkeys.ContainsKey(x.Value.variable2)
                ).ToList();

                foreach (var solvedMonkey in solvedMonkeys)
                {
                    allOtherMonkeys.Remove(solvedMonkey.Key);

                    var v1 = allMonkeys[solvedMonkey.Value.variable1];
                    var v2 = allMonkeys[solvedMonkey.Value.variable2];

                    var result = solvedMonkey.Value.op switch
                    {
                        '+' => v1 + v2,
                        '*' => v1 * v2,
                        '/' => v1 / v2,
                        '-' => v1 - v2
                    };

                    allMonkeys.Add(solvedMonkey.Key, result);
                }
            }
        }
    }
}
