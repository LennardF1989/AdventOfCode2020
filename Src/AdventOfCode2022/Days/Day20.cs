using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day20
    {
        private sealed record Entry(int OriginalIndex, long Value);

        private static Entry ParseInput(string x, int i, long decryptionKey = 1) => 
            new Entry(i, int.Parse(x) * decryptionKey);

        private static long GetAtCircular(List<Entry> entries, int index) => entries[index % entries.Count].Value;

        private static void OutputDebug(Entry current, List<Entry> entries) => Logger.Debug(
            $"{current?.Value.ToString() ?? "Initial"}:\n{string.Join(", ", entries.Select(x => x.Value))}\n"
        );

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day20_Test.txt")
                .ReadAllLines("Content\\Day20.txt")
                .Select((x, i) => ParseInput(x, i))
                .ToList();

            Mixing(lines);

            var zeroIndex = lines.IndexOf(lines.Find(x => x.Value == 0));

            var answer =
                GetAtCircular(lines, zeroIndex + 1000) +
                GetAtCircular(lines, zeroIndex + 2000) +
                GetAtCircular(lines, zeroIndex + 3000);

            Logger.Info($"Day 20A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day20_Test.txt")
                .ReadAllLines("Content\\Day20.txt")
                .Select((x, i) => ParseInput(x, i, 811589153))
                .ToList();

            for (var i = 0; i < 10; i++)
            {
                Mixing(lines);
            }

            var zeroIndex = lines.IndexOf(lines.Find(x => x.Value == 0));

            var answer =
                GetAtCircular(lines, zeroIndex + 1000) +
                GetAtCircular(lines, zeroIndex + 2000) +
                GetAtCircular(lines, zeroIndex + 3000);

            Logger.Info($"Day 20B: {answer}");
        }

        private static void Mixing(List<Entry> entries)
        {
            //OutputDebug(null, entries);

            var entryLookup = entries.ToDictionary(x => x.OriginalIndex);

            for (var i = 0; i < entries.Count; i++)
            {
                var current = entryLookup[i];

                //NOTE: Skip 0
                if (current.Value == 0)
                {
                    continue;
                }

                var currentIndex = entries.IndexOf(current);
                
                //NOTE: Remove it immediately so we don't have to deal with off-by-one errors
                entries.RemoveAt(currentIndex);
                
                //BUGFIX: Cast the result of the modulo to int...
                var newIndex = (int)((currentIndex + current.Value) % entries.Count);

                //NOTE: The result of module can be negative
                if (newIndex < 0)
                {
                    newIndex = entries.Count + newIndex;
                }

                entries.Insert(newIndex, current);
                    
                //OutputDebug(current, entries);
            }
        }
    }
}
