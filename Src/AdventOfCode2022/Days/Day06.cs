using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day06
    {
        public static void StartA()
        {
            var packets = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                ;

            foreach (var packet in packets)
            {
                var answer = FindStartOfMessageMarker(packet, 4);

                Logger.Info($"Day 6A: {answer}");
            }
        }

        public static void StartB()
        {
            var packets = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                ;

            foreach (var packet in packets)
            {
                var answer = FindStartOfMessageMarker2(packet, 14);

                Logger.Info($"Day 6B: {answer}");
            }
        }

        private static int FindStartOfMessageMarker(string packet, int length)
        {
            var queue = new Queue<char>();

            for (var index = 0; index < packet.Length; index++)
            {
                char c = packet[index];

                queue.Enqueue(c);

                var chars = new HashSet<char>(queue.ToArray());

                if (chars.Count == length)
                {
                    return index + 1;
                }

                if (queue.Count == length)
                {
                    queue.Dequeue();
                }
            }

            return -1;
        }

        //NOTE: Alternative version - Slower
        private static int FindStartOfMessageMarker2(string packet, int length)
        {
            for (var i = 0; i < packet.Length; i++)
            {
                if (packet.Skip(i).Take(length).ToHashSet().Count == length)
                {
                    return i + 1;
                }
            }

            return -1;
        }
    }
}
