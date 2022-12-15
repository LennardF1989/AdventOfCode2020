using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day15
    {
        enum State
        {
            Empty = 0,
            ScanArea,
            Beacon,
            Sensor
        }

        class Grid
        {
            public int MinX { get; set; }
            public int MaxX { get; set; }
            public int MinY { get; set; }
            public int MaxY { get; set; }
            
            public Dictionary<(int x, int y), State> Map { get; set; }

            public State GetState(int x, int y)
            {
                return Map.GetValueOrDefault((x, y), State.Empty);
            }

            public void SetCoord(int x, int y, State state)
            {
                Map[(x, y)] = state;
            }

            public void DrawGrid()
            {
                var stringBuilder = new StringBuilder();
                
                for (var y = MinY; y < MaxY; y++)
                {
                    for (var x = MinX; x < MaxX; x++)
                    {
                        var state = GetState(x, y);

                        switch (state)
                        {
                            case State.Empty:
                                stringBuilder.Append('.');
                                break;

                            case State.ScanArea:
                                stringBuilder.Append('#');
                                break;

                            case State.Sensor:
                                stringBuilder.Append('S');
                                break;

                            case State.Beacon:
                                stringBuilder.Append('B');
                                break;
                        }

                    }

                    stringBuilder.AppendLine();
                }

                Logger.Debug(stringBuilder.ToString());
            }
        }

        class Rectangle
        {
            public int MinX { get; set; } 
            public int MaxX { get; set; } 
            public int MinY { get; set; } 
            public int MaxY { get; set; }

            public Rectangle(int minX, int maxX, int minY, int maxY)
            {
                MinX = minX;
                MaxX = maxX;
                MinY = minY;
                MaxY = maxY;
            }
        }
        
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content//Day15_Test.txt")
                .ReadAllLines("Content//Day15.txt")
                .Select(x =>
                {
                    var match = Regex.Match(x, "Sensor at x=(.*?), y=(.*?): closest beacon is at x=(.*?), y=(.*?)$");

                    var sensor = (
                        x: int.Parse(match.Groups[1].Value),
                        y: int.Parse(match.Groups[2].Value)
                    );

                    var beacon = (
                        x: int.Parse(match.Groups[3].Value),
                        y: int.Parse(match.Groups[4].Value)
                    );
                    
                    var distance = Math.Abs(beacon.x - sensor.x) + Math.Abs(beacon.y - sensor.y);

                    return (
                        sensor,
                        beacon,
                        distance
                    );
                })
                //.ReadAllLines("Content\\Day15.txt")
                .ToList();

            var allCoords = lines.SelectMany(o =>
            {
                var coord = o.sensor;

                var up = (coord.x, y: coord.y - o.distance);
                var left = (x: coord.x - o.distance, coord.y);
                var right = (x: coord.x + o.distance, coord.y);
                var down = (coord.x, y: coord.y + o.distance);

                return new[] { up, left, right, down };
            }).ToList();

            var minX = allCoords.Min(x => x.x);
            var maxX = allCoords.Max(x => x.x);
            var minY = allCoords.Min(x => x.y);
            var maxY = allCoords.Max(x => x.y);

            var map = new Dictionary<(int x, int y), State>();

            var rectangles = lines.Select(o =>
            {
                var coord = o.sensor;

                return (line: o, new Rectangle(
                    coord.x - o.distance, 
                    coord.x + o.distance, 
                    coord.y - o.distance,
                    coord.y + o.distance
                ));
            }).ToList();

            //const int maxSize = 20;
            const int maxSize = 4_000_000;

            void Test(int targetY)
            {
                var grid = new State[maxSize + 1];

                var result = rectangles
                    .Where(x => x.Item2.MinY <= targetY && x.Item2.MaxY >= targetY)
                    .ToList();

                foreach (var o in result)
                {
                    var line = o.line;

                    var size = 1;
                    var x = line.sensor.x;

                    for (var y = line.sensor.y - line.distance; y < line.sensor.y; y++)
                    {
                        if (y == targetY)
                        {
                            for (var i = 0; i < size && x + i < maxSize; i++)
                            {
                                if (x + i < 0)
                                {
                                    continue;
                                }

                                grid[x + i] = State.ScanArea;
                            }
                        }

                        x--;
                        size += 2;
                    }

                    for (var y = line.sensor.y; y <= line.sensor.y + line.distance; y++)
                    {
                        if (y == targetY)
                        {
                            for (var i = 0; i < size && x + i < maxSize; i++)
                            {
                                if (x + i < 0)
                                {
                                    continue;
                                }

                                grid[x + i] = State.ScanArea;
                            }
                        }

                        x++;
                        size -= 2;
                    }
                }

                for (int x = 0; x < maxSize; x++)
                {
                    var state = grid[x];

                    if (state == State.Empty)
                    {
                        Logger.Debug($"Found: {x},{targetY} => {x * 4000000 + targetY}");
                    }
                }
            }

            for (var y = 0; y < maxSize; y += 10000)
            {
                Parallel.For(0, 10000, Test);
            }

            foreach (var line in lines)
            {
                //grid.SetCoord(line.beacon.x, line.beacon.y, State.Beacon);
                //grid.SetCoord(line.sensor.x, line.sensor.y, State.Sensor);
            }

            //grid.DrawGrid();

            var answer = 0;

            Logger.Info($"Day 15A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content//Day15_Test.txt")
                .ReadAllLines("Content//Day15.txt")
                .Select(x =>
                {
                    var match = Regex.Match(x, "Sensor at x=(.*?), y=(.*?): closest beacon is at x=(.*?), y=(.*?)$");

                    var sensor = (
                        x: int.Parse(match.Groups[1].Value),
                        y: int.Parse(match.Groups[2].Value)
                    );

                    var beacon = (
                        x: int.Parse(match.Groups[3].Value),
                        y: int.Parse(match.Groups[4].Value)
                    );

                    var distance = ManhattanDistance(sensor, beacon);

                    return (
                        sensor,
                        beacon,
                        distance
                    );
                })
                .ToList()
                ;

            //const int maxSize = 20;
            const long maxSize = 4_000_000;

            var hashSets = new List<HashSet<(int x, int y)>>();

            foreach (var line in lines)
            {
                var hashSet = new HashSet<(int x, int y)>();

                for (var xOffset = -line.distance - 1; xOffset <= line.distance + 1; xOffset++)
                {
                    var x = line.sensor.x + xOffset;
                    var yOffset = (line.distance - Math.Abs(xOffset)) + 1;

                    hashSet.Add((x, line.sensor.y + yOffset));
                    hashSet.Add((x, line.sensor.y - yOffset));
                }

                hashSets.Add(hashSet);
            }

            long answer = 0;

            foreach (var hashSet in hashSets)
            {
                foreach (var coord in hashSet)
                {
                    if (coord.x <= 0 || coord.y <= 0 || coord.x >= maxSize || coord.y >= maxSize)
                    {
                        continue;
                    }

                    var overlap = lines.Any(x => ManhattanDistance(x.sensor, coord) <= x.distance);

                    if (overlap)
                    {
                        continue;
                    }

                    Logger.Debug(coord);

                    answer = (coord.x * maxSize) + coord.y;
                    //goto answer;
                }
            }

            //Too low: 1543906354
            //Long! 12413999391794
            answer:
            Logger.Info($"Day 15B: {answer}");
        }

        public static int ManhattanDistance((int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        public static void DrawGrid(HashSet<(int x, int y)> hashSet)
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y <= 20; y++)
            {
                for (var x = 0; x <= 20; x++)
                {
                    var state = hashSet.Contains((x, y));

                    stringBuilder.Append(state ? '#' : '.');
                }

                stringBuilder.AppendLine();
            }

            Logger.Debug(stringBuilder.ToString());
        }
    }
}
