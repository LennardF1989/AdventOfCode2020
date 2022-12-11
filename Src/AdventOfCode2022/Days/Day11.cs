using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day11
    {
        public class Item
        {
            //BUGFIX: Has to be long for Part 2
            public long WorryLevel { get; set; }

            public Item(int worryLevel)
            {
                WorryLevel = worryLevel;
            }
        }

        public class Monkey
        {
            public List<Item> Items { get; }
            public readonly int TestValue;

            public int LCM { get; set; }
            public long InspectCount { get; private set; }

            private readonly int _monkeyNumber;
            private readonly string[] _operation;
            private readonly int _testTrue;
            private readonly int _testFalse;

            public Monkey(int monkeyNumber, List<Item> items, string[] operation, int testValue, int testTrue, int testFalse)
            {
                _monkeyNumber = monkeyNumber;
                Items = items;
                _operation = operation;
                TestValue = testValue;
                _testTrue = testTrue;
                _testFalse = testFalse;
            }

            public List<(Item item, int target)> RunRound(bool part1)
            {
                var throwItems = new List<(Item item, int target)>();

                //Logger.Debug($"Monkey {_monkeyNumber}:");

                foreach (var item in Items)
                {
                    Inspect(item, part1);
                    throwItems.Add((item, Test(item)));
                }

                return throwItems;
            }

            public void Inspect(Item item, bool part1)
            {
                InspectCount++;

                //Logger.Debug($" Monkey inspects an item with worry level of {item.WorryLevel}.");

                item.WorryLevel = Operation(item);

                //Logger.Debug($"   Worry level is changed to {item.WorryLevel}");

                if (part1)
                {
                    item.WorryLevel = (int)Math.Floor(item.WorryLevel / 3f);
                    //Logger.Debug($"   Monkey gets bored with item. Worry level is divided by 3 to {item.WorryLevel}.");
                }
                else
                {
                    item.WorryLevel %= LCM;
                    //Logger.Debug($"   Monkey gets bored with item. Worry level is {item.WorryLevel}.");
                }
            }

            public long Operation(Item item)
            {
                var stack = new Stack<long>();

                foreach (var c in _operation)
                {
                    if (c == "old")
                    {
                        stack.Push(item.WorryLevel);
                    }
                    else if (c == "*")
                    {
                        var left = stack.Pop();
                        var right = stack.Pop();

                        return left * right;
                    }
                    else if (c == "+")
                    {
                        var left = stack.Pop();
                        var right = stack.Pop();

                        return left + right;
                    }
                    else
                    {
                        stack.Push(int.Parse(c));
                    }
                }

                return 0;
            }

            public int Test(Item item)
            {
                if (item.WorryLevel % TestValue == 0)
                {
                    //Logger.Debug($"   Current worry level is divisible by {TestValue}.");
                    //Logger.Debug($"   Item with worry level {item.WorryLevel} is thrown to monkey {_testTrue}.");

                    return _testTrue;
                }

                //Logger.Debug($"   Current worry level is not divisible by {TestValue}.");
                //Logger.Debug($"   Item with worry level {item.WorryLevel} is thrown to monkey {_testFalse}.");

                return _testFalse;
            }
        }

        public static void StartA()
        {
            var lines = File
                //.ReadAllText("Content\\Day11_Test.txt")
                .ReadAllText("Content\\Day11.txt")
                ;

            var monkeys = GetMonkeys(lines);

            for (var i = 0; i < 20; i++)
            {
                foreach (var monkey in monkeys)
                {
                    var result = monkey.RunRound(true);

                    foreach (var valueTuple in result)
                    {
                        monkey.Items.Remove(valueTuple.item);
                        monkeys[valueTuple.target].Items.Add(valueTuple.item);
                    }
                }

                //Logger.Debug($"Round {i + 1}: {string.Join(" ", monkeys.Select(x => x.InspectCount))}");
            }

            var results = monkeys.Select(x => x.InspectCount).OrderByDescending(x => x).ToList();

            var answer = results[0] * results[1];

            Logger.Info($"Day 11A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllText("Content\\Day11_Test.txt")
                .ReadAllText("Content\\Day11.txt")
                ;

            var monkeys = GetMonkeys(lines);

            //Disclaimer: Someone helped me figure this out...
            var lcm = monkeys.Aggregate(1, (current, monkey) => current * monkey.TestValue);

            foreach (var monkey in monkeys)
            {
                monkey.LCM = lcm;
            }

            for (var i = 0; i < 10000; i++)
            {
                foreach (var monkey in monkeys)
                {
                    var result = monkey.RunRound(false);

                    foreach (var valueTuple in result)
                    {
                        monkey.Items.Remove(valueTuple.item);
                        monkeys[valueTuple.target].Items.Add(valueTuple.item);
                    }
                }

                //Logger.Debug($"Round {i + 1}: {string.Join(" ", monkeys.Select(x => x.InspectCount))}");
            }

            var results = monkeys
                .Select(x => x.InspectCount)
                .OrderByDescending(x => x)
                .ToList();

            var answer = results[0] * results[1];

            Logger.Info($"Day 11B: {answer}");
        }

        private static List<Monkey> GetMonkeys(string input)
        {
            return input
                .Split("\r\n\r\n")
                .Select(x =>
                {
                    var lines = x.Split("\r\n");

                    var name = lines[0].Split(":")[0];
                    var monkeyNumber = int.Parse(name.Split(" ").Last());
                    var items = lines[1].Split(":")[1].Split(",").Select(y => new Item(int.Parse(y))).ToList();
                    var operation = lines[2].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var operationPostfix = new[] { operation[2], operation[4], operation[3] };
                    var divisibleBy = int.Parse(lines[3].Split(":")[1].Split(" ").Last());
                    var ifTrue = int.Parse(lines[4].Split(":")[1].Split(" ").Last());
                    var ifFalse = int.Parse(lines[5].Split(":")[1].Split(" ").Last());

                    return new Monkey(monkeyNumber, items, operationPostfix, divisibleBy, ifTrue, ifFalse);
                })
                .ToList();
        }
    }
}
