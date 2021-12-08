using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day23
    {
        class LinkedCups
        {
            public Cup First { get; set; }
            public Cup Current { get; set; }

            public List<Cup> CupsByNumber { get; set; }

            public LinkedCups(int count)
            {
                CupsByNumber = new List<Cup>(Enumerable.Repeat<Cup>(null, count));
            }

            public List<Cup> DisconnectNext(int count)
            {
                List<Cup> cups = new List<Cup>();

                var firstCup = Current.Next;
                var lastCup = Current.Next;

                while (true)
                {
                    cups.Add(lastCup);

                    if (lastCup == Current)
                    {
                        Current = lastCup.Next;
                    }

                    count--;

                    if (count <= 0)
                    {
                        break;
                    }

                    lastCup = lastCup.Next;
                }

                firstCup.Previous.Next = lastCup.Next;
                lastCup.Next.Previous = firstCup.Previous;

                firstCup.Previous = null;
                lastCup.Next = null;

                return cups;
            }

            public void AttachNext(Cup target, List<Cup> cups)
            {
                var firstCup = cups[0];
                var lastCup = cups[^1];

                var temp = target.Next;

                target.Next = firstCup;
                firstCup.Previous = target;

                lastCup.Next = temp;
                temp.Previous = lastCup;
            }

            public string GetCups()
            {
                var stringBuilder = new StringBuilder();

                var cup = First;

                while (cup != null)
                {
                    if (cup == Current)
                    {
                        stringBuilder.Append($"({cup.Number}) ");
                    }
                    else
                    {
                        stringBuilder.Append($"{cup.Number} ");
                    }

                    cup = cup.Next;

                    if (cup == First)
                    {
                        break;
                    }
                }

                return stringBuilder.ToString();
            }
        }

        class Cup
        {
            public int Number { get; set; }
            public Cup Previous { get; set; }
            public Cup Next { get; set; }

            public override string ToString()
            {
                return Number.ToString();
            }
        }

        public static void StartA()
        {
            //var input = File.ReadAllText("Content\\Day23_Test.txt");
            var input = File.ReadAllText("Content\\Day23.txt");

            var linkedCups = ParseInput(input);

            //DoMoves(linkedCups, 10, true);
            DoMoves(linkedCups, 100, true);
            var output = GetOutputSequence(linkedCups, 1);

            Logger.Info($"Day 23A: {output}");
        }

        public static void StartB()
        {
            //var input = File.ReadAllText("Content\\Day23_Test.txt");
            var input = File.ReadAllText("Content\\Day23.txt");

            var linkedCups = ParseInput2(input);
            DoMoves(linkedCups, 10_000_000, false);

            long result1 = linkedCups.CupsByNumber[1].Next.Number;
            long result2 = linkedCups.CupsByNumber[1].Next.Next.Number;
            long result = result1 * result2;

            Logger.Debug($"{result1} * {result2} = {result}");

            Logger.Info($"Day 23B: {result}");
        }

        private static LinkedCups ParseInput(string input)
        {
            LinkedCups linkedCups = new LinkedCups(10);
            
            Cup lastCup = null;

            foreach (char c in input)
            {
                var cup = new Cup
                {
                    Number = (int) char.GetNumericValue(c)
                };

                linkedCups.CupsByNumber[cup.Number] = cup;

                if (lastCup != null)
                {
                    lastCup.Next = cup;
                    cup.Previous = lastCup;
                }

                if (linkedCups.First == null)
                {
                    linkedCups.First = cup;
                    linkedCups.Current = cup;
                }

                lastCup = cup;
            }

            linkedCups.First.Previous = lastCup;
            lastCup.Next = linkedCups.First;

            return linkedCups;
        }

        private static LinkedCups ParseInput2(string input)
        {
            LinkedCups linkedCups = new LinkedCups(1_000_000 + 1);

            Cup lastCup = null;

            int count = 0;
            
            foreach (char c in input)
            {
                var cup = new Cup
                {
                    Number = (int) char.GetNumericValue(c)
                };

                linkedCups.CupsByNumber[cup.Number] = cup;

                if (lastCup != null)
                {
                    lastCup.Next = cup;
                    cup.Previous = lastCup;
                }

                if (linkedCups.First == null)
                {
                    linkedCups.First = cup;
                    linkedCups.Current = cup;
                }

                lastCup = cup;

                count++;
            }

            for (int i = count + 1; i <= 1_000_000; i++)
            {
                var cup = new Cup
                {
                    Number = i
                };

                linkedCups.CupsByNumber[cup.Number] = cup;

                if (lastCup != null)
                {
                    lastCup.Next = cup;
                    cup.Previous = lastCup;
                }

                lastCup = cup;
            }

            linkedCups.First.Previous = lastCup;
            lastCup.Next = linkedCups.First;

            return linkedCups;
        }

        private static void DoMoves(LinkedCups linkedCups, int moves, bool debugging)
        {
            linkedCups.Current = linkedCups.First;
            var move = 0;

            while (linkedCups.Current != null && move < moves)
            {
                move++;

                if (debugging)
                {
                    Logger.Debug($"-- move {move} --");
                    Logger.Debug($"cups: {linkedCups.GetCups()}");
                }

                int activeCup = linkedCups.Current.Number;

                var pickedCups = linkedCups.DisconnectNext(3);

                int destination = activeCup - 1;

                while (destination <= 0 || pickedCups.Any(x => x.Number == destination))
                {
                    destination--;

                    if (destination <= 0)
                    {
                        destination = linkedCups.CupsByNumber.Count - 1;
                    }
                }

                if (debugging)
                {
                    Logger.Debug($"pick up: {string.Join(", ", pickedCups)}");
                    Logger.Debug($"destination: {destination}");
                    Logger.Debug(string.Empty);
                }

                linkedCups.AttachNext(linkedCups.CupsByNumber[destination], pickedCups);

                linkedCups.Current = linkedCups.Current.Next;
            }

            if (debugging)
            {
                Logger.Debug("-- final --");
                Logger.Debug($"cups: {linkedCups.GetCups()}");
            }
        }

        private static StringBuilder GetOutputSequence(LinkedCups linkedCups, int startNumber)
        {
            var firstCup = linkedCups.CupsByNumber[startNumber];

            var cup = firstCup.Next;
            var output = new StringBuilder();

            while (cup != null)
            {
                output.Append(cup.Number);

                cup = cup.Next;

                if (cup == firstCup)
                {
                    break;
                }
            }

            return output;
        }
    }
}
