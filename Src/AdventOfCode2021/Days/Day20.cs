using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day20
    {
        private static readonly List<(int x, int y)> _surroundingCoords = new()
        {
            (-1, -1),
            (0, -1),
            (1, -1),
            (-1, 0),
            (0, 0),
            (1, 0),
            (-1, 1),
            (0, 1),
            (1, 1)
        };

        public static void Start()
        {
            var lines = File
                //.ReadAllText("Content\\Day20_Test.txt")
                .ReadAllText("Content\\Day20.txt")
                .Split("\r\n\r\n")
                ;

            var algorithm = lines[0];

            var image = lines[1]
                .Split("\r\n")
                .Select(x => x.Select(y => y == '#' ? (byte)1 : (byte)0).ToList())
                .ToList();

            var hashSet = new HashSet<(int x, int y)>();
            var startX = 0;
            var startY = 0;
            var width = image.Count;
            var height = image[0].Count;

            for (var y = 0; y < image.Count; y++)
            {
                for (var x = 0; x < image[y].Count; x++)
                {
                    if (image[y][x] == 1)
                    {
                        hashSet.Add((x, y));
                    }
                }
            }

            Print(hashSet);

            hashSet = EnhanceImage(hashSet, algorithm, ref startX, ref startY, ref width, ref height, 2);
            int answer1 = hashSet.Count(x => x.x >= startX && x.x <= width && x.y >= startY && x.y <= height);

            hashSet = EnhanceImage(hashSet, algorithm, ref startX, ref startY, ref width, ref height, 48);
            int answer2 = hashSet.Count(x => x.x >= startX && x.x <= width && x.y >= startY && x.y <= height);

            Logger.Info($"Day 20A: {answer1}");
            Logger.Info($"Day 20B: {answer2}");
        }

        private static HashSet<(int x, int y)> EnhanceImage(
            HashSet<(int x, int y)> hashSet, string algorithm,
            ref int startX, ref int startY, ref int width, ref int height,
            int repeat
        )
        {
            const int offset = 3;

            for (int i = 0; i < repeat; i++)
            {
                startX -= 1;
                startY -= 1;
                width += 1;
                height += 1;

                var newHashSet = new HashSet<(int x, int y)>();

                for (var y = startY - offset; y < height + offset; y++)
                {
                    for (var x = startX - offset; x < width + offset; x++)
                    {
                        var xCopy = x;
                        var yCopy = y;
                        var hashSetCopy = hashSet;

                        var binary = GetInt(_surroundingCoords.Select(c => GetValue(hashSetCopy, c.x + xCopy, c.y + yCopy)));

                        if (algorithm[binary] == '#')
                        {
                            newHashSet.Add((xCopy, yCopy));
                        }
                    }
                }

                hashSet = newHashSet;

                if ((i + 1) % 2 == 0)
                {
                    var startXCopy = startX;
                    var startWidthCopy = width;
                    var startYCopy = startY;
                    var startHeightCopy = height;

                    hashSet = new HashSet<(int x, int y)>(
                        hashSet.Where(x => x.x >= startXCopy && x.x <= startWidthCopy && x.y >= startYCopy && x.y <= startHeightCopy)
                    );
                }

                Print(hashSet);
            }

            return hashSet;
        }

        private static byte GetValue(HashSet<(int x, int y)> lines, int x, int y)
        {
            return lines.Contains((x, y)) ? (byte)1 : (byte)0;
        }

        private static int GetInt(IEnumerable<byte> bits)
        {
            int result = 0;

            foreach (var b in bits)
            {
                result = result << 1 | b;
            }

            return result;
        }

        private static void Print(HashSet<(int x, int y)> hashSet)
        {
            const int offset = 3;

            var minX = hashSet.Min(x => x.x) - offset;
            var minY = hashSet.Min(x => x.y) - offset;
            var maxX = hashSet.Max(x => x.x) + offset + 1;
            var maxY = hashSet.Max(x => x.y) + offset + 1;

            var stringBuilder = new StringBuilder();

            for (var y = minY; y < maxY; y++)
            {
                for (var x = minX; x < maxX; x++)
                {
                    stringBuilder.Append(hashSet.Contains((x, y)) ? '#' : '.');
                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder);
        }
    }
}
