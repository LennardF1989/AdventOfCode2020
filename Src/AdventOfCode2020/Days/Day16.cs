using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day16
    {
        private class ParsedInput
        {
            public Dictionary<string, long[]> TicketClass { get; set; }
            public long[] YourTicket { get; set; }
            public List<long[]> NearbyTickets { get; set; }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day16_Test.txt");
            var lines = File.ReadAllLines("Content\\Day16.txt");
            var parsedInput = ParseLines(lines);

            long sum = 0;

            foreach (var nearbyTicket in parsedInput.NearbyTickets)
            {
                foreach (var value in nearbyTicket)
                {
                    if (!IsInAnyRange(value, parsedInput))
                    {
                        sum += value;
                    }
                }
            }

            Logger.Info($"Day 16A: {sum}");
        }

        private static bool IsInAnyRange(long value, ParsedInput parsedInput)
        {
            return parsedInput.TicketClass.Values.Any(x => IsInRange(value, x));
        }

        private static bool IsInRange(long value, long[] ranges)
        {
            return (value >= ranges[0] && value <= ranges[1]) || (value >= ranges[2] && value <= ranges[3]);
        }

        private static ParsedInput ParseLines(string[] lines)
        {
            ParsedInput parsedInput = new ParsedInput
            {
                TicketClass = new Dictionary<string, long[]>(),
                NearbyTickets = new List<long[]>()
            };

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("your ticket:"))
                {
                    parsedInput.YourTicket = lines[++i]
                        .Split(",")
                        .Select(long.Parse)
                        .ToArray();
                }
                else if (line.StartsWith("nearby tickets:"))
                {
                    i++;

                    for (; i < lines.Length; i++)
                    {
                        line = lines[i];

                        parsedInput.NearbyTickets.Add(line.Split(",").Select(long.Parse).ToArray());
                    }
                }
                else
                {
                    var line1 = line.Split(":");
                    var line2 = line1[1].Split("or");
                    var line3A = line2[0].Split("-");
                    var line3B = line2[1].Split("-");

                    parsedInput.TicketClass.Add(
                        line1[0],
                        new[] { long.Parse(line3A[0]), long.Parse(line3A[1]), long.Parse(line3B[0]), long.Parse(line3B[1]) }
                    );
                }
            }

            return parsedInput;
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day16_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day16.txt");
            var parsedInput = ParseLines(lines);

            var validNearbyTickets = parsedInput
                .NearbyTickets
                .Where(nearbyTicket => nearbyTicket
                    .All(x => IsInAnyRange(x, parsedInput))
                )
                .ToList();

            List<List<List<string>>> potentialClassesForTickets = new List<List<List<string>>>();

            foreach (var values in validNearbyTickets)
            {
                var currentTicket = new List<List<string>>();
                potentialClassesForTickets.Add(currentTicket);

                for (var position = 0; position < values.Length; position++)
                {
                    currentTicket.Add(new List<string>());

                    foreach (var kvp in parsedInput.TicketClass)
                    {
                        var value = values[position];

                        if (IsInRange(value, kvp.Value))
                        {
                            currentTicket[position].Add(kvp.Key);
                        }
                    }
                }
            }

            var totalCountTickers = potentialClassesForTickets.Count;
            var totalNumberOfFields = validNearbyTickets[0].Length;

            var filteredClasses = new List<List<string>>();

            for (int i = 0; i < totalNumberOfFields; i++)
            {
                filteredClasses.Add(new List<string>());

                var allPossibleFields = potentialClassesForTickets
                    .SelectMany(x => x[i])
                    .Distinct()
                    .ToList();

                var allPossibleClassesForField = potentialClassesForTickets
                    .Select(x => x[i])
                    .ToList();

                foreach (var allPossibleField in allPossibleFields)
                {
                    var count = allPossibleClassesForField.Count(x => x.Contains(allPossibleField));

                    if (count == totalCountTickers)
                    {
                        filteredClasses[i].Add(allPossibleField);

                        Logger.Debug($"Possible field for #{i}: {allPossibleField}");
                    }
                }
            }

            List<string> finalFields = new List<string>();
            finalFields.AddRange(Enumerable.Repeat(string.Empty, totalNumberOfFields));

            while (true)
            {
                var field = filteredClasses
                    .Select((x, i) => new { x, i })
                    .Where(x => x.x.Count == 1)
                    .Select(x => new { x = x.x[0], x.i })
                    .FirstOrDefault();

                if (field == null)
                {
                    break;
                }

                finalFields[field.i] = field.x;

                filteredClasses.ForEach(x => x.Remove(field.x));
            }

            for (var i = 0; i < finalFields.Count; i++)
            {
                var finalField = finalFields[i];
                Logger.Debug($"Field for #{i}: {finalField}");
            }

            var indices = finalFields
                .Select((x, i) => new {x, i})
                .Where(x => x.x.Contains("departure"))
                .Select(x => x.i)
                .ToList();

            var fieldValues = parsedInput.YourTicket
                .Where((x, i) => indices.Contains(i))
                .Select(x => x)
                .ToList();

            var sum = fieldValues.FirstOrDefault();

            for (int i = 1; i < fieldValues.Count; i++)
            {
                sum *= fieldValues[i];
            }

            Logger.Info($"Day 16B: {sum}");
        }
    }
}
