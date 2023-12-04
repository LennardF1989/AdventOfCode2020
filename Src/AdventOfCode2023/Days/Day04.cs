using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2023.Days
{
    public static class Day04
    {
        public static void StartA()
        {
            var cards = ParseLines();

            var answer = 0;

            foreach (var (_, winningNumbers, potentialNumbers) in cards)
            {
                potentialNumbers.IntersectWith(winningNumbers);

                var amount = (int) Math.Pow(2, potentialNumbers.Count - 1);

                answer += amount;
            }

            Logger.Info($"Day 4A: {answer}");
        }

        public static void StartB()
        {
            var cards = ParseLines();

            var simplifiedCards = new List<int>();
            var simplifiedCardsRepeat = new List<int>();

            foreach (var (_, winningNumbers, potentialNumbers) in cards)
            {
                potentialNumbers.IntersectWith(winningNumbers);

                simplifiedCards.Add(potentialNumbers.Count);
                simplifiedCardsRepeat.Add(1);
            }

            for (var i = 0; i < simplifiedCards.Count; i++)
            {
                var card = simplifiedCards[i];
                var repeat = simplifiedCardsRepeat[i];

                for (var j = 0; j < card; j++)
                {
                    simplifiedCardsRepeat[i + 1 + j] += repeat;
                }
            }

            var answer = simplifiedCardsRepeat.Sum();

            Logger.Info($"Day 4B: {answer}");
        }

        private static List<(int cardId, HashSet<int> winningNumbers, HashSet<int> potentialNumbers)> ParseLines()
        {
            return File
                //.ReadAllLines("Content\\Day04_Test.txt")
                .ReadAllLines("Content\\Day04.txt")
                .SelectList(x =>
                {
                    var card = x.Split(":");
                    var cardId = card[0].Split(" ")[^1].ToInteger();
                    var numbers = card[1]
                        .Split("|", true)
                        .SelectList(y => y
                            .Split(" ", true, true)
                            .SelectHashSet(z => z.ToInteger())
                        );

                    var winningNumbers = numbers[0];
                    var potentialNumbers = numbers[1];

                    return (
                        cardId,
                        winningNumbers,
                        potentialNumbers
                    );
                });
        }
    }
}
