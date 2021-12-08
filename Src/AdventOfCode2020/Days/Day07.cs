using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public class Day07
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day07_Test.txt");
            var lines = File.ReadAllLines("Content\\Day07.txt");

            Dictionary<string, List<Tuple<int, string>>> bags = new Dictionary<string, List<Tuple<int, string>>>();

            foreach (string line in lines)
            {
                string[] splittedLine = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                string bagType = $"{splittedLine[0]} {splittedLine[1]}";

                //NOTE: Skip 2 and 3
                bags.Add(bagType, new List<Tuple<int, string>>());

                if (splittedLine[4] == "no" && splittedLine[5] == "other" && splittedLine[6] == "bags.")
                {
                    continue;
                }

                int i = 3;

                do
                {
                    i += 1;

                    int numberOfBags = int.Parse(splittedLine[i]);
                    string otherBagType = $"{splittedLine[i + 1]} {splittedLine[i + 2]}";

                    bags[bagType].Add(new Tuple<int, string>(numberOfBags, otherBagType));

                    i = i + 3;
                } while (i < splittedLine.Length && splittedLine[i].EndsWith(","));
            }

            int answer = FindShinyGold(bags);

            Logger.Info($"Day 7A: {answer}");
        }

        public static int FindShinyGold(Dictionary<string, List<Tuple<int, string>>> bags)
        {
            var visited = new List<string>();
            var goldBags = new List<string>();

            foreach (var bag in bags)
            {
                var currentBag = bag.Key;

                var stack = new Stack<string>();

                stack.Push(currentBag);

                var parentMap = new Dictionary<string, string>();
                parentMap[currentBag] = null;

                while (stack.Any())
                {
                    currentBag = stack.Pop();

                    if (goldBags.Contains(currentBag))
                    {
                        //NOTE: Trace up
                        var current = currentBag;
                        while (current != null)
                        {
                            Logger.Debug(current);
                            goldBags.Add(current);

                            current = parentMap[current];
                        }

                        continue;
                    }

                    if (visited.Contains(currentBag))
                    {
                        continue;
                    }

                    visited.Add(currentBag);

                    var otherBags = bags[currentBag].ToList();
                    otherBags.Reverse();

                    if (otherBags.Any(x => x.Item2 == "shiny gold"))
                    {
                        //NOTE: Struck gold!
                        Logger.Debug("Found gold!");

                        //NOTE: Trace up
                        var current = currentBag;
                        while (current != null)
                        {
                            Logger.Debug(current);
                            goldBags.Add(current);

                            current = parentMap[current];
                        }

                        continue;
                    }

                    foreach (var otherBag in otherBags)
                    {
                        stack.Push(otherBag.Item2);

                        parentMap[otherBag.Item2] = currentBag;
                    }
                }
            }

            var result = goldBags
                .Distinct()
                .ToList();

            return result.Count;
        }

        class Bag
        {
            public string Name { get; set; }

            public List<Tuple<Bag, int>> ContainsBags { get; } = new List<Tuple<Bag, int>>();

            public int NumberOfBags()
            {
                int total = 0;

                foreach (var bag in ContainsBags)
                {
                    total += bag.Item2 + bag.Item2 * bag.Item1.NumberOfBags();
                }

                return total;
            }
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day07_Test.txt");
            //var lines = File.ReadAllLines("Content\\Day07_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day07.txt");

            Dictionary<string, Bag> bags = new Dictionary<string, Bag>();

            foreach (string line in lines)
            {
                string[] splittedLine = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                string bagType = $"{splittedLine[0]} {splittedLine[1]}";

                //NOTE: Skip 2 and 3
                if (!bags.ContainsKey(bagType))
                {
                    bags.Add(bagType, new Bag
                    {
                        Name = bagType
                    });
                }

                var currentBag = bags[bagType];

                if (splittedLine[4] == "no" && splittedLine[5] == "other" && splittedLine[6] == "bags.")
                {
                    continue;
                }

                int i = 3;

                do
                {
                    i += 1;

                    int numberOfBags = int.Parse(splittedLine[i]);
                    string otherBagType = $"{splittedLine[i + 1]} {splittedLine[i + 2]}";

                    if (!bags.ContainsKey(otherBagType))
                    {
                        bags.Add(otherBagType, new Bag
                        {
                            Name = otherBagType
                        });
                    }

                    currentBag.ContainsBags.Add(new Tuple<Bag, int>(bags[otherBagType], numberOfBags));

                    i = i + 3;
                } while (i < splittedLine.Length && splittedLine[i].EndsWith(","));
            }

            int answer = HowManyBags(bags.Values.ToList(), "shiny gold");

            Logger.Info($"Day 7B: {answer}");
        }

        private static int HowManyBags(List<Bag> bags, string targetBag)
        {
            var currentBag = bags.First(x => x.Name == targetBag);

            return currentBag.NumberOfBags();
        }
    }
}
