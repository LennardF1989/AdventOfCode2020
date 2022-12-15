﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            public int OffsetX { get; set; }
            public int OffsetY { get; set; }

            //public State[,] Map { get; set; }
            public Dictionary<(int x, int y), State> Map2 { get; set; }

            public (int x, int y) GetCoord(int x, int y)
            {
                //return (x + OffsetX, y + OffsetY);
                return (x, y);
            }

            public State GetState(int x, int y)
            {
                var coord = GetCoord(x, y);

                //return Map[coord.y, coord.x];
                return Map2.GetValueOrDefault(coord, State.Empty);
            }

            public void SetCoord(int x, int y, State state)
            {
                var coord = GetCoord(x, y);

                //Map[coord.y, coord.x] = state;
                Map2[coord] = state;
            }

            public void DrawGrid()
            {
                var stringBuilder = new StringBuilder();

                //for (var y = 0; y < Map.GetLength(0); y++)
                for (var y = MinY; y < MaxY; y++)
                {
                    //for (var x = 0; x < Map.GetLength(1); x++)
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

        public static void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day15_Test.txt")
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

                    //var distance = (int)Math.Sqrt(((beacon.x - sensor.x) * (beacon.x - sensor.x)) +
                    //                         ((beacon.y - sensor.y) * (beacon.y - sensor.y)));
                    var distance = Math.Max(beacon.x, beacon.y) - 1;

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

            var offsetX = (minX < 0) ? Math.Abs(minX) : minX;
            var offsetY = (minY < 0) ? Math.Abs(minY) : minY;

            var map = new State[
                Math.Abs(minX) + Math.Abs(maxX) + 10,
                Math.Abs(minY) + Math.Abs(maxY) + 10
            ];

            var map2 = new Dictionary<(int x, int y), State>();

            var grid = new Grid
            {
                MinX = minX,
                MaxX = maxX,
                MinY = minY,
                MaxY = maxY,
                OffsetX = offsetX,
                OffsetY = offsetY,
                //Map = map
                Map2 = map2
            };

            foreach (var line in lines)
            {
                grid.SetCoord(line.beacon.x, line.beacon.y, State.Beacon);
                grid.SetCoord(line.sensor.x, line.sensor.y, State.Sensor);

                grid.DrawGrid();

                var size = 1;
                var x = line.sensor.x;

                for (var y = line.sensor.y - line.distance; y < line.sensor.y; y++)
                {
                    for (var i = 0; i < size; i++)
                    {
                        if (grid.GetState(x + i, y) != State.Empty)
                        {
                            continue;
                        }

                        grid.SetCoord(x + i, y, State.ScanArea);
                    }

                    grid.DrawGrid();

                    x--;
                    size += 2;
                }

                for (var y = line.sensor.y; y <= line.sensor.y + line.distance; y++)
                {
                    for (var i = 0; i < size; i++)
                    {
                        if (grid.GetState(x + i, y) != State.Empty)
                        {
                            continue;
                        }

                        grid.SetCoord(x + i, y, State.ScanArea);
                    }

                    x++;
                    size -= 2;
                }

                grid.DrawGrid();
            }

            var count = 0;
            for (int x = 0; x < grid.MaxY; x++)
            {
                if (grid.GetState(x, 10) == State.ScanArea)
                {
                    count++;
                }
            }

            var answer = count;

            Logger.Info($"Day 15A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day15_Test.txt")
                //.ReadAllLines("Content\\Day.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 15B: {answer}");
        }
    }
}
