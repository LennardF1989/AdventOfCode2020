using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day21
    {
        private class Line
        {
            public List<string> Ingredients { get; set; }
            public List<string> Allergens { get; set; }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day21_Test.txt");
            var lines = File.ReadAllLines("Content\\Day21.txt");

            var parsedLines = ParseLines(lines);
            var _ = GetTranslations(parsedLines);

            var sanityCheck = parsedLines.Sum(x => x.Allergens.Count);
            Logger.Debug($"Allergens left: {sanityCheck} (Should be 0)");

            var remainingIngredients = parsedLines
                .SelectMany(x => x.Ingredients)
                .Count();

            Logger.Info($"Day 21A: {remainingIngredients}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day21_Test.txt");
            var lines = File.ReadAllLines("Content\\Day21.txt");

            var parsedLines = ParseLines(lines);
            var translations = GetTranslations(parsedLines);

            var sortedTranslations = translations
                .OrderBy(x => x.Value)
                .Select(x => x.Key)
                .ToList();

            var answer = string.Join(",", sortedTranslations);

            Logger.Info($"Day 21B: {answer}");
        }

        private static List<Line> ParseLines(string[] lines)
        {
            var parsedLines = new List<Line>();

            foreach (var line in lines)
            {
                var line1 = line.Split("(");

                var ingredients = line1[0]
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .ToList();

                var containsLength = "contains".Length;
                var allergens = line1[1]
                    .Substring(containsLength, line1[1].Length - containsLength - 1)
                    .Split(",")
                    .Select(x => x.Trim())
                    .ToList();

                var parsedLine = new Line
                {
                    Ingredients = ingredients,
                    Allergens = allergens
                };

                parsedLines.Add(parsedLine);
            }

            return parsedLines;
        }

        private static Dictionary<string, string> GetTranslations(List<Line> parsedLines)
        {
            var ignoredLines = new List<Line>();
            var translations = new Dictionary<string, string>();

            while (true)
            {
                var first = parsedLines.FirstOrDefault(x => !ignoredLines.Contains(x) && x.Allergens.Count == 1);

                if (first == null)
                {
                    break;
                }

                var firstAllergen = first.Allergens[0];

                var candidates = parsedLines
                    .Where(x => x != first && x.Allergens.Contains(firstAllergen))
                    .ToList();

                List<string> foundIngredients = new List<string>();
                foreach (var firstIngredient in first.Ingredients)
                {
                    var result = candidates.All(x => x.Ingredients.Contains(firstIngredient));

                    if (result)
                    {
                        foundIngredients.Add(firstIngredient);

                        //NOTE: Optimization
                        if (foundIngredients.Count > 1)
                        {
                            break;
                        }
                    }
                }

                if (foundIngredients.Count == 1)
                {
                    ignoredLines.Clear();

                    var firstIngredient = foundIngredients[0];

                    Logger.Debug($"{firstIngredient} => {firstAllergen}");
                    translations.Add(firstIngredient, firstAllergen);

                    parsedLines.ForEach(x =>
                    {
                        x.Ingredients.Remove(firstIngredient);
                        x.Allergens.Remove(firstAllergen);
                    });
                }
                else
                {
                    ignoredLines.Add(first);
                }
            }

            return translations;
        }
    }
}