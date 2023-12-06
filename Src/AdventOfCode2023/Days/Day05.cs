using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2023.Days
{
    public static class Day05
    {
        public static void StartA()
        {
            var input = ParseInput();

            var locations = new List<long>();

            foreach (var seed in input.seeds)
            {
                var value = seed;
                var kvp = input.maps.First(x => x.Key.from == "seed");

            nextMap:
                var result = kvp.Value
                    .Find(x => x.IsSourceInRange(value))
                    ?.GetDestination(value) ?? value;

                if (result > -1)
                {
                    value = result;
                }

                kvp = input.maps.FirstOrDefault(x => x.Key.from == kvp.Key.to);

                if (kvp.Key != null)
                {
                    goto nextMap;
                }

                locations.Add(value);
            }

            var answer = locations.Min();

            Logger.Info($"Day 5A: {answer}");
        }

        public static void StartB()
        {
            var input = ParseInput();

            var locations = new ConcurrentBag<long>();

            var mapsAsArray = input.maps
                .Select(x => x.Value.OrderBy(y => y.sourceStart).ToArray())
                .ToArray();

            Parallel.For(0, input.seeds.Count / 2, (i) =>
            {
                Logger.Debug($"Thread {i} - Start");

                var min = long.MaxValue;

                var startSeed = input.seeds[i * 2];
                var length = input.seeds[(i * 2) + 1];

                for (var j = 0; j < length; j++)
                {
                    var mapIndex = 0;
                    var value = startSeed + j;
                    var kvp = mapsAsArray[mapIndex];

                nextMap:
                    var first = kvp[0];
                    var last = kvp[^1];

                    if (value >= first.sourceStart && value < last.sourceEnd)
                    {
                        value = Array.Find(kvp, x => x.IsSourceInRange(value))?.GetDestination(value) ?? value;
                    }

                    mapIndex++;

                    if (mapIndex < mapsAsArray.Length)
                    {
                        kvp = mapsAsArray[mapIndex];

                        goto nextMap;
                    }

                    min = Math.Min(value, min);
                }

                Logger.Debug($"Thread {i} - Done");

                locations.Add(min);
            });

            var answer = locations.Min();

            Logger.Info($"Day 5B: {answer}");
        }

        private static Input ParseInput()
        {
            var lines = File
                //.ReadAllText("Content\\Day05_Test.txt")
                .ReadAllText("Content\\Day05.txt")
                .Split("\r\n\r\n")
                ;

            const string seedsPrefix = "seeds: ";

            List<long> seeds = null;
            var maps = new Dictionary<Map, List<MapEntry>>();

            foreach (var line in lines)
            {
                if (line.StartsWith(seedsPrefix))
                {
                    seeds = line[seedsPrefix.Length..]
                        .Split(" ", true)
                        .SelectList(x => x.ToLong());
                }
                else
                {
                    var map = line.Split(":");
                    var mapNames = map[0].Split(" ")[0].Split("-");
                    var mapFrom = mapNames[0];
                    var mapTo = mapNames[^1];
                    var mapEntries = map[1]
                        .Split("\r\n", true, true)
                        .SelectList(x =>
                        {
                            var result = x.Split(" ").SelectList(y => y.ToLong());

                            return new MapEntry(
                                result[0] - result[1], 
                                result[1], 
                                result[1] + result[2]
                            );
                        });

                    maps.Add(new Map(mapFrom, mapTo), mapEntries);
                }
            }

            return new Input(seeds, maps);
        }

        record Input(List<long> seeds, Dictionary<Map, List<MapEntry>> maps);

        record Map(string from, string to);

        record MapEntry(long destinationDifference, long sourceStart, long sourceEnd)
        {
            public bool IsSourceInRange(long source)
            {
                return source >= sourceStart && source < sourceEnd;
            }

            public long GetDestination(long source)
            {
                return source + destinationDifference;
            }
        }
    }
}
