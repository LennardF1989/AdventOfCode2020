using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2023.Days
{
    public static class Day07
    {
        private sealed record Hand(string OriginalCards, List<Card> Cards, int BitAmount, int JokerCount)
        {
            public EHandKind Kind { get; set; }

            public override string ToString()
            {
                return $"{OriginalCards} ({Kind} | {JokerCount})";
            }
        }

        private sealed record Card(char Type, int Count);

        public enum EHandKind
        {
            None = 0,
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard,
        }

        private static readonly Dictionary<char, int> _cardMapping = new()
        {
            { 'A', 0 },
            { 'K', 1 },
            { 'Q', 2 },
            { JokerCard, 3 },
            { 'T', 4 },
            { '9', 5 },
            { '8', 6 },
            { '7', 7 },
            { '6', 8 },
            { '5', 9 },
            { '4', 10 },
            { '3', 11 },
            { '2', 12 }
        };

        private const char JokerCard = 'J';

        public static void StartA()
        {
            var hands = ParseInput(false);
            hands.Sort(CompareHands);

            var answer = hands.Select((x, i) => (i + 1) * x.BitAmount).Sum();

            Logger.Info($"Day 7A: {answer}");
        }

        public static void StartB()
        {
            var hands = ParseInput(true);

            _cardMapping['J'] = 13;

            hands.Sort(CompareHands);

            var answer = hands.Select((x, i) => (i + 1) * x.BitAmount).Sum();

            Logger.Info($"Day 7B: {answer}");
        }

        private static List<Hand> ParseInput(bool partB)
        {
            var hands = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectList(x =>
                {
                    var hand = x.Split(" ");

                    var cards = hand[0]
                        .GroupBy(y => y)
                        .Select(y => new Card(y.Key, y.Count()))
                        .OrderByDescending(y => y.Count)
                        .ToList();

                    var bitAmount = hand[1].ToInteger();

                    return new Hand(
                        hand[0],
                        cards,
                        bitAmount,
                        cards.Find(y => y.Type == 'J')?.Count ?? 0
                    );
                });

            foreach (var hand in hands)
            {
                var i = 0;

                var card1 = hand.Cards[i];
                var highest1 = card1.Count;

                i++;

                if (partB)
                {
                    if (card1.Type == JokerCard && i < hand.Cards.Count)
                    {
                        highest1 += hand.Cards[i].Count;

                        i++;
                    }
                    else if (card1.Type != JokerCard)
                    {
                        highest1 += hand.JokerCount;
                    }
                }

                hand.Kind = highest1 switch
                {
                    5 => EHandKind.FiveOfAKind,
                    4 => EHandKind.FourOfAKind,
                    3 => hand.Cards[i].Count == 2 
                        ? EHandKind.FullHouse 
                        : EHandKind.ThreeOfAKind,
                    2 => hand.Cards[i].Count == 2 
                        ? EHandKind.TwoPair 
                        : EHandKind.OnePair,
                    _ => EHandKind.HighCard
                };
            }

            return hands;
        }

        private static int CompareHands(Hand a, Hand b)
        {
            //Hand A wins
            if (a.Kind < b.Kind)
            {
                return 1;
            }

            //Hand B wins
            if (a.Kind > b.Kind)
            {
                return -1;
            }

            return CompareCards(a.OriginalCards, b.OriginalCards);
        }

        private static int CompareCards(string a, string b)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var c1 = a[i];
                var c2 = b[i];

                if (c1 == c2)
                {
                    continue;
                }

                //Hand A wins
                if (_cardMapping[c1] < _cardMapping[c2])
                {
                    return 1;
                }

                //Hand B wins
                return -1;
            }

            return 0;
        }
    }
}
