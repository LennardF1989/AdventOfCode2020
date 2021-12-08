using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day17
    {
        enum CubeState
        {
            Active = 0,
            Inactive
        }

        private class InfiniteGrid3D
        {
            private readonly Dictionary<string, CubeState> _points;
            private Vector4 _minBounds;
            private Vector4 _maxBounds;

            public InfiniteGrid3D()
            {
                _points = new Dictionary<string, CubeState>();
                _minBounds = new Vector4();
                _maxBounds = new Vector4();
            }

            public InfiniteGrid3D(InfiniteGrid3D grid)
            {
                _points = grid._points.ToDictionary(x => x.Key, x => x.Value);
                _minBounds = grid._minBounds;
                _maxBounds = grid._maxBounds;
            }

            public CubeState GetPoint(int x, int y, int z, int w)
            {
                var key = $"{x},{y},{z},{w}";

                return _points.TryGetValue(key, out var slice)
                    ? slice
                    : CubeState.Inactive;
            }

            public void SetPoint(int x, int y, int z, int w, CubeState cubeState)
            {
                var key = $"{x},{y},{z},{w}";

                if (_points.ContainsKey(key))
                {
                    _points[key] = cubeState;
                }
                else
                {
                    _points.Add(key, cubeState);
                }

                _minBounds.X = Math.Min(x, _minBounds.X);
                _minBounds.Y = Math.Min(y, _minBounds.Y);
                _minBounds.Z = Math.Min(z, _minBounds.Z);
                _minBounds.W = Math.Min(w, _minBounds.W);
                _maxBounds.X = Math.Max(x, _maxBounds.X);
                _maxBounds.Y = Math.Max(y, _maxBounds.Y);
                _maxBounds.Z = Math.Max(y, _maxBounds.Z);
                _maxBounds.W = Math.Max(w, _maxBounds.W);
            }

            public Vector4 GetMinBounds()
            {
                return _minBounds;
            }

            public Vector4 GetMaxBounds()
            {
                return _maxBounds;
            }

            public int GetTotalPoints(CubeState active)
            {
                return _points.Count(x => x.Value == active);
            }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day17_Test.txt");
            var lines = File.ReadAllLines("Content\\Day17.txt");

            InfiniteGrid3D grid = new InfiniteGrid3D();

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    grid.SetPoint(x, y, 0, 0, c == '#' ? CubeState.Active : CubeState.Inactive);
                }
            }

            PrintGrid(grid, 0, 0);

            for (int i = 0; i < 6; i++)
            {
                grid = RunCycle(grid, false);
            }

            int numberOfActiveCubes = grid.GetTotalPoints(CubeState.Active);

            Logger.Info($"Day 17A: {numberOfActiveCubes}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day17_Test.txt");
            var lines = File.ReadAllLines("Content\\Day17.txt");

            InfiniteGrid3D grid = new InfiniteGrid3D();

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    grid.SetPoint(x, y, 0, 0, c == '#' ? CubeState.Active : CubeState.Inactive);
                }
            }

            PrintGrid(grid, 0, 0);

            for (int i = 0; i < 6; i++)
            {
                grid = RunCycle(grid, true);
            }

            int numberOfActiveCubes = grid.GetTotalPoints(CubeState.Active);

            Logger.Info($"Day 17B: {numberOfActiveCubes}");
        }

        private static InfiniteGrid3D RunCycle(InfiniteGrid3D grid, bool useFourthDimension)
        {
            var gridClone = new InfiniteGrid3D(grid);

            var minBounds = grid.GetMinBounds();
            minBounds.X--;
            minBounds.Y--;
            minBounds.Z--;

            var maxBounds = grid.GetMaxBounds();
            maxBounds.X++;
            maxBounds.Y++;
            maxBounds.Z++;

            if (useFourthDimension)
            {
                minBounds.W--;
                maxBounds.W++;
            }

            for (int w = (int) minBounds.W; w <= (int) maxBounds.W; w++)
            {
                for (int z = (int)minBounds.Z; z <= (int)maxBounds.Z; z++)
                {
                    for (int y = (int)minBounds.Y; y <= (int)maxBounds.Y; y++)
                    {
                        for (int x = (int)minBounds.X; x <= (int)maxBounds.X; x++)
                        {
                            int numberOfActiveNeighbours = GetNumberOfActiveNeighbours(grid, x, y, z, w, useFourthDimension);

                            var cubeState = grid.GetPoint(x, y, z, w);

                            if (cubeState == CubeState.Active && (numberOfActiveNeighbours < 2 || numberOfActiveNeighbours > 3))
                            {
                                gridClone.SetPoint(x, y, z, w, CubeState.Inactive);
                            }
                            else if (cubeState == CubeState.Inactive && numberOfActiveNeighbours == 3)
                            {
                                gridClone.SetPoint(x, y, z, w, CubeState.Active);
                            }
                        }
                    }

                    PrintGrid(gridClone, z, w);
                }
            }

            return gridClone;
        }

        private static int GetNumberOfActiveNeighbours(InfiniteGrid3D grid, int x, int y, int z, int w, bool useFourthDimension)
        {
            int activeNeighbours = 0;

            void InnerLoop(int newW)
            {
                for (int newZ = z - 1; newZ <= z + 1; newZ++)
                {
                    for (int newY = y - 1; newY <= y + 1; newY++)
                    {
                        for (int newX = x - 1; newX <= x + 1; newX++)
                        {
                            if (newX == x && newY == y && z == newZ && w == newW)
                            {
                                continue;
                            }

                            //Logger.Debug($"{newX} {newY} {newZ} {newW}");

                            if (grid.GetPoint(newX, newY, newZ, newW) == CubeState.Active)
                            {
                                activeNeighbours++;
                            }
                        }
                    }
                }
            }

            if (useFourthDimension)
            {
                for (int newW = w - 1; newW <= w + 1; newW++)
                {
                    InnerLoop(newW);
                }
            }
            else
            {
                InnerLoop(0);
            }

            return activeNeighbours;
        }

        private static void PrintGrid(InfiniteGrid3D grid, int z, int w)
        {
            Logger.Debug($"z={z}, w={w}");
            
            var minBounds = grid.GetMinBounds();
            var maxBounds = grid.GetMaxBounds();

            for (int y = (int)minBounds.Y; y <= (int)maxBounds.Y; y++)
            {
                string line = string.Empty;

                for (int x = (int)minBounds.X; x <= (int)maxBounds.X; x++)
                {
                    line += grid.GetPoint(x, y, z, w) == CubeState.Active ? "#" : ".";
                }

                Logger.Debug(line);
            }

            Logger.Debug(string.Empty);
        }
    }
}