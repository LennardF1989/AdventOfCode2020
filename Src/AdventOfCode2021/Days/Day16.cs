using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day16
    {
        private static long _versions;
        
        public static void Start()
        {
            var lines = File
                //.ReadAllLines("Content\\Day16_Test.txt")
                .ReadAllLines("Content\\Day16.txt")
                .Select(StringToByteArray)
                .ToList();

            var bits = new List<int>();

            foreach (var line in lines)
            {
                foreach (var b in line)
                {
                    var b0 = (b >> 0) & 1;
                    var b1 = (b >> 1) & 1;
                    var b2 = (b >> 2) & 1;
                    var b3 = (b >> 3) & 1;
                    var b4 = (b >> 4) & 1;
                    var b5 = (b >> 5) & 1;
                    var b6 = (b >> 6) & 1;
                    var b7 = (b >> 7) & 1;

                    //NOTE: Reverse the order
                    bits.Add(b7);
                    bits.Add(b6);
                    bits.Add(b5);
                    bits.Add(b4);
                    bits.Add(b3);
                    bits.Add(b2);
                    bits.Add(b1);
                    bits.Add(b0);
                }
            }

            //57340821780 is too low
            var answer = ReadPacket(bits.ToArray(), out _);

            Logger.Info($"Day 16A: {_versions}");
            Logger.Info($"Day 16B: {answer}");
        }

        private static long ReadPacket(int[] bits, out int consumedOffset)
        {
            int offset = 0;

            int version = GetInt(bits.Skip(offset).Take(3).ToArray());
            offset += 3;

            //HACK: Sorry...
            _versions += version;

            int packetType = GetInt(bits.Skip(offset).Take(3).ToArray());
            offset += 3;

            if (packetType == 4)
            {
                var result = GetLiteralValue(bits.Skip(offset).ToArray(), out var literalOffset);

                consumedOffset = offset + literalOffset;

                return result;
            }

            var results = new List<long>();

            int lengthTypeId = bits.Skip(offset).Take(1).First();
            offset++;

            if (lengthTypeId == 0)
            {
                int length = GetInt(bits.Skip(offset).Take(15).ToArray());
                offset += 15;

                int totalOffsetConsumed = 0;

                while (totalOffsetConsumed < length)
                {
                    var result = ReadPacket(bits.Skip(offset).ToArray(), out var literalOffset);
                    offset += literalOffset;

                    totalOffsetConsumed += literalOffset;

                    results.Add(result);
                }
            }
            else if (lengthTypeId == 1)
            {
                int length = GetInt(bits.Skip(offset).Take(11).ToArray());
                offset += 11;

                for (var p = 0; p < length; p++)
                {
                    var result = ReadPacket(bits.Skip(offset).ToArray(),  out var literalOffset);
                    offset += literalOffset;

                    results.Add(result);
                }
            }

            consumedOffset = offset;

            switch (packetType)
            {
                //Sum
                case 0:
                    return results.Sum();
                case 1:
                {
                    return results.Aggregate(1L, (current, i) => current * i);
                }
                case 2:
                    return results.Min();
                case 3:
                    return results.Max();
                case 5:
                    return results[0] > results[1] ? 1 : 0;
                case 6:
                    return results[0] < results[1] ? 1 : 0;
                case 7:
                    return results[0] == results[1] ? 1 : 0;
                default:
                    return 0;
            }
        }

        private static int GetInt(int[] bits)
        {
            int result = 0;

            foreach (var b in bits)
            {
                result = result << 1 | b;
            }

            return result;
        }

        private static long GetLiteralValue(int[] bits, out int consumedOffset)
        {
            int offset = 0;
            List<long> groups = new List<long>();

            bool hasMoreGroups;
            do
            {
                hasMoreGroups = bits.Skip(offset).Take(1).First() == 1;
                offset++;

                var group = GetInt(bits.Skip(offset).Take(4).ToArray());
                offset += 4;

                groups.Add(group);
            } while (hasMoreGroups);

            long value = 0;

            foreach (var group in groups)
            {
                value = value << 4 | group;
            }

            consumedOffset = offset;

            return value;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}