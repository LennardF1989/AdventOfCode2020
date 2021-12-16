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
            var testLines = File
                .ReadAllLines("Content\\Day16_Test.txt")
                .Select(x =>
                {
                    var split = x.Split("=");

                    return (input: StringToByteArray(split[0]), expected: int.Parse(split[1]));
                })
                .ToList();

            foreach (var testLine in testLines)
            {
                var testBits = ByteArrayToBitsArray(testLine.input);
                var actual  = ReadPacket(testBits, out _);

                Logger.Debug($"{testLine.expected} == {actual} => {actual == testLine.expected}");
            }

            var lines = StringToByteArray(
                File.ReadAllText("Content\\Day16.txt")
            );

            var bits = ByteArrayToBitsArray(lines);
            var answer = ReadPacket(bits, out _);

            Logger.Info($"Day 16A: {_versions}");
            Logger.Info($"Day 16B: {answer}");
        }

        private static long ReadPacket(byte[] bits, out int consumedOffset)
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
                    var result = ReadPacket(bits.Skip(offset).ToArray(), out var literalOffset);
                    offset += literalOffset;

                    results.Add(result);
                }
            }

            consumedOffset = offset;

            return packetType switch
            {
                0 => results.Sum(),
                1 => results.Aggregate(1L, (a, b) => a * b),
                2 => results.Min(),
                3 => results.Max(),
                5 => results[0] > results[1] ? 1 : 0,
                6 => results[0] < results[1] ? 1 : 0,
                7 => results[0] == results[1] ? 1 : 0,
                _ => 0
            };
        }

        private static int GetInt(byte[] bits)
        {
            int result = 0;

            foreach (var b in bits)
            {
                result = result << 1 | b;
            }

            return result;
        }

        private static long GetLiteralValue(byte[] bits, out int consumedOffset)
        {
            int offset = 0;
            long value = 0;

            bool hasMoreGroups;
            do
            {
                hasMoreGroups = bits.Skip(offset).Take(1).First() == 1;
                offset++;

                var group = GetInt(bits.Skip(offset).Take(4).ToArray());
                offset += 4;

                value = value << 4 | (uint)group;
            } while (hasMoreGroups);

            consumedOffset = offset;

            return value;
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        private static byte[] ByteArrayToBitsArray(byte[] bytes)
        {
            var bits = new byte[bytes.Length * 8];

            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];

                //NOTE: Reverse the order
                bits[i * 8 + 0] = (byte)((b >> 7) & 1);
                bits[i * 8 + 1] = (byte)((b >> 6) & 1);
                bits[i * 8 + 2] = (byte)((b >> 5) & 1);
                bits[i * 8 + 3] = (byte)((b >> 4) & 1);
                bits[i * 8 + 4] = (byte)((b >> 3) & 1);
                bits[i * 8 + 5] = (byte)((b >> 2) & 1);
                bits[i * 8 + 6] = (byte)((b >> 1) & 1);
                bits[i * 8 + 7] = (byte)((b >> 0) & 1);
            }

            return bits;
        }
    }
}