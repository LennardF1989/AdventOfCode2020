using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day13
    {
        public static void StartA()
        {
            var lines = File.ReadAllLines("Content\\Day13.txt");

            int estimateTimestamp = int.Parse(lines[0]);
            List<int> departures = lines[1]
                .Split(",")
                .Select(x =>
                {
                    if (int.TryParse(x, out var e))
                    {
                        return e;
                    }

                    return 0;
                })
                .Where(x => x > 0)
                .ToList();

            //Departure time, Bus IDs 
            Dictionary<int, List<int>> departureTimes = new Dictionary<int, List<int>>();

            foreach (var busId in departures)
            {
                int amount = (int) Math.Ceiling((float) estimateTimestamp / busId);
                int departureTime = amount * busId;

                if (!departureTimes.ContainsKey(departureTime))
                {
                    departureTimes[departureTime] = new List<int>();
                }

                departureTimes[departureTime].Add(busId);
            }

            var minTime = departureTimes.Keys.Min();
            var minBusId = departureTimes[minTime].First();

            var answer = (minTime - estimateTimestamp) * minBusId;

            Logger.Info($"Day 13A: {answer}");
        }

        //Disclaimer: I cheated with this one, would have never found Chinese Remainder Theorem myself.
        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day13_Test.txt");
            var lines = File.ReadAllLines("Content\\Day13.txt");

            List<int> departures = lines[1]
                .Split(",")
                .Select(x =>
                {
                    if (int.TryParse(x, out var e))
                    {
                        return e;
                    }

                    return 0;
                })
                .ToList();

            long answer = ChineseRemainderTheorem(
                departures
                    .Where(x => x > 0)
                    .Select(x => (long)x)
                    .ToArray(),
                departures
                    .Select((x, i) => new { i, x})
                    .Where(x => x.x > 0)
                    .Select(x => (long)(x.x - x.i) % x.x) //(Bus ID - Position) % Bus ID
                    .ToArray()
                );

            Logger.Info($"Day 13B: {answer}");
        }

        //Based on: https://rosettacode.org/wiki/Chinese_remainder_theorem#C.23
        private static long ChineseRemainderTheorem(long[] n, long[] a)
        {
            static long ModularMultiplicativeInverse(long a, long mod)
            {
                long b = a % mod;

                for (int x = 1; x < mod; x++)
                {
                    if ((b * x) % mod == 1)
                    {
                        return x;
                    }
                }

                return 1;
            }

            long prod = n.Aggregate(1, (long i, long j) => i * j);
            long sm = 0;

            for (int i = 0; i < n.Length; i++)
            {
                var p = prod / n[i];

                sm += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
            }

            return sm % prod;
        }

        public static void StartB2()
        {
            //var lines = File.ReadAllLines("Content\\Day13_Test.txt");
            var lines = File.ReadAllLines("Content\\Day13.txt");

            List<int> departures = lines[1]
                .Split(",")
                .Select(x =>
                {
                    if (int.TryParse(x, out var e))
                    {
                        return e;
                    }

                    return 0;
                })
                .ToList();
            
            long t = 0;
            long increment = departures[0];

            int offset = 1;
            List<long> foundPatterns = new List<long>
            {
                departures[0]
            };

            while (offset < departures.Count)
            {
                if (departures[offset] == 0)
                {
                    offset++;

                    continue;
                }

                t += increment;

                if ((t + offset) % departures[offset] == 0)
                {
                    foundPatterns.Add(departures[offset]);
                    increment = DetermineLeastCommonMultiple(foundPatterns);
                    offset++;

                    Logger.Debug(increment);
                }
            }
            
            Logger.Info($"Day 12B: {t}");
        }

        //Based on: https://stackoverflow.com/questions/13569810/least-common-multiple
        private static long DetermineLeastCommonMultiple(List<long> numbers)
        {
            static long GreatestCommonFactor(long a, long b)
            {
                while (b != 0)
                {
                    long temp = b;
                    b = a % b;
                    a = temp;
                }
                return a;
            }

            static long LeastCommonMultiple(long a, long b)
            {
                return (a / GreatestCommonFactor(a, b)) * b;
            }

            long lastLcm = LeastCommonMultiple(numbers[^2], numbers[^1]);

            for (int i = numbers.Count - 3; i >= 0; i--)
            {
                lastLcm = LeastCommonMultiple(numbers[i], lastLcm);
            }

            return lastLcm;
        }

        public static void StartB3()
        {
            //var lines = File.ReadAllLines("Content\\Day13_Test.txt");
            var lines = File.ReadAllLines("Content\\Day13.txt");

            List<int> departures = lines[1]
                .Split(",")
                .Select(x =>
                {
                    if (int.TryParse(x, out var e))
                    {
                        return e;
                    }

                    return 0;
                })
                .ToList();
            
            long t = 0;
            long increment = departures[0];
            int offset = 1;

            //Loop while we still have Bus ID's to check
            while (offset < departures.Count)
            {
                //If the current offset and at a 'x'-location, increment it.
                if (departures[offset] == 0)
                {
                    offset++;

                    continue;
                }

                //Increment the current time with the latest determined increment
                t += increment;

                //Check if the current time results in a repeated pattern at the offset
                if ((t + offset) % departures[offset] != 0)
                {
                    continue;
                }

                //Multiply the current increment with the Bus ID
                increment *= departures[offset];

                //Increment the offset so we can start looking for the next repeating pattern
                offset++;
            }
            
            Logger.Info($"Day 12B: {t}");
        }
    }
}
