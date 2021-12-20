using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2021.Days
{
    public static class Day19
    {
        private class Scanner
        {
            public List<Vector3> Beacons { get; set; }
            public List<Vector3> Positions { get; set; }
        }

        private enum EAxis
        {
            X = 0,
            Z = 1
        }

        //Source: https://stackoverflow.com/questions/54496824/how-to-compute-all-orientations-of-a-cube-by-rotating-without-repeating-orienta
        //Source: https://math.stackexchange.com/questions/3165534/what-are-all-possible-orientations-of-the-platonic-solids
        private static readonly EAxis[] _rotationAxis =
        {
            EAxis.X,
            EAxis.X,
            EAxis.Z,
            EAxis.X,
            EAxis.X,
            EAxis.Z
        };

        private static readonly Matrix4x4 _rotateX = Matrix4x4.CreateRotationX((float)Math.PI / 2);
        private static readonly Matrix4x4 _rotateY = Matrix4x4.CreateRotationY((float)Math.PI / 2);
        private static readonly Matrix4x4 _rotateZ = Matrix4x4.CreateRotationZ((float)Math.PI / 2);

        public static void Start()
        {
            var scanners = File
                //.ReadAllText("Content\\Day19_Test.txt")
                .ReadAllText("Content\\Day19.txt")
                .Split("\r\n\r\n")
                .Select(x =>
                {
                    var beacons = x
                        .Split("\r\n")
                        .Skip(1)
                        .Select(y =>
                        {
                            var ps = y.Split(",").Select(int.Parse).ToArray();

                            return new Vector3(ps[0], ps[1], ps[2]);
                        })
                        .ToList();

                    return new Scanner
                    {
                        Beacons = beacons,
                        Positions = new List<Vector3>()
                    };
                })
                .ToList();

            var result = ReduceScanners(scanners);

            int largestDistance = 0;

            foreach (var a in result.Positions)
            {
                foreach (var b in result.Positions)
                {
                    if (a == b)
                    {
                        continue;
                    }

                    int distance = ManhattanDistance(a, b);

                    if (distance > largestDistance)
                    {
                        largestDistance = distance;
                    }
                }
            }

            Logger.Info($"Day 19A: {result.Beacons.Count}");
            Logger.Info($"Day 19B: {largestDistance}");
        }

        private static int ManhattanDistance(Vector3 a, Vector3 b)
        {
            var x = Math.Abs(a.X - b.X);
            var y = Math.Abs(a.Y - b.Y);
            var z = Math.Abs(a.Z - b.Z);

            return (int)(x + y + z);
        }

        private static Scanner ReduceScanners(List<Scanner> scanners)
        {
            while (true)
            {
                Logger.Debug($"Scanners left: {scanners.Count}");

                var scannedPairs = new HashSet<(int p1, int p2)>();
                var removeScanners = new List<Scanner>();

                for (int i = 0; i < scanners.Count; i++)
                {
                    for (int i2 = 0; i2 < scanners.Count; i2++)
                    {
                        if (i == i2 || scannedPairs.Contains((i, i2)))
                        {
                            continue;
                        }

                        scannedPairs.Add((i, i2));
                        scannedPairs.Add((i2, i));

                        var result = DetermineOverlap(scanners[i], scanners[i2]);

                        if (result != null)
                        {
                            Logger.Debug($"Found pair {i} => {i2}");

                            scanners[i] = result;
                            removeScanners.Add(scanners[i2]);
                        }
                    }

                    if (removeScanners.Count == 0)
                    {
                        continue;
                    }

                    removeScanners.ForEach(x => scanners.Remove(x));

                    if (scanners.Count == 1)
                    {
                        Logger.Debug("Only 1 scanner left, done!");

                        return scanners[0];
                    }

                    break;
                }   
            }
        }

        private static Scanner DetermineOverlap(Scanner scanner1, Scanner scanner2)
        {
            var scannedPairs = new HashSet<(int p1, int p2)>
            {
                (0, 0)
            };

            for (int s1 = 0; s1 < scanner1.Beacons.Count; s1++)
            {
                for (int s2 = 0; s2 < scanner2.Beacons.Count; s2++)
                {
                    if (s1 == s2 || scannedPairs.Contains((s1, s2)))
                    {
                        continue;
                    }

                    scannedPairs.Add((s1, s2));
                    scannedPairs.Add((s2, s1));

                    var vector1 = scanner1.Beacons[s1];
                    var matrix1 = Matrix4x4.CreateTranslation(-vector1.X, -vector1.Y, -vector1.Z);
                    var matrix1Inverse = Matrix4x4.CreateTranslation(vector1.X, vector1.Y, vector1.Z);

                    var listOfPoints1 = scanner1.Beacons.Select(x => matrix1.Transform(x)).ToList();

                    var vector2 = scanner2.Beacons[s2];
                    var correctedVector2 = vector2;

                    var matrix2 = Matrix4x4.CreateTranslation(-vector2.X, -vector2.Y, -vector2.Z);

                    var listOfPoints2 = scanner2.Beacons.Select(x => matrix2.Transform(x)).ToList();

                    foreach (var axis in _rotationAxis)
                    {
                        for (int i2 = 0; i2 < 4; i2++)
                        {
                            var overlappingPoints = listOfPoints1.Overlaps(listOfPoints2);

                            if (overlappingPoints.Count >= 12)
                            {
                                goto found;
                            }

                            correctedVector2 = _rotateY.Transform(correctedVector2);

                            listOfPoints2 = listOfPoints2
                                .Select(x => _rotateY.Transform(x))
                                .ToList();
                        }

                        switch (axis)
                        {
                            case EAxis.X:
                                correctedVector2 = _rotateX.Transform(correctedVector2);
                                listOfPoints2 = listOfPoints2.Select(x => _rotateX.Transform(x)).ToList();
                                break;
                            case EAxis.Z:
                                correctedVector2 = _rotateZ.Transform(correctedVector2);
                                listOfPoints2 = listOfPoints2.Select(x => _rotateZ.Transform(x)).ToList();
                                break;
                        }
                    }

                    continue;

                    found:
                    var newScanner = listOfPoints2
                        .Select(x => matrix1Inverse.Transform(x))
                        .ToList();

                    var newPosition = newScanner[s2];
                    var relativePosition = correctedVector2;

                    var scannerPosition = new Vector3(
                        newPosition.X - relativePosition.X, 
                        newPosition.Y - relativePosition.Y, 
                        newPosition.Z - relativePosition.Z
                    );

                    return new Scanner
                    {
                        Beacons = new HashSet<Vector3>(scanner1.Beacons.Concat(newScanner)).ToList(),
                        Positions = new List<Vector3>(scanner1.Positions)
                        {
                            scannerPosition
                        }
                    };
                }
            }

            return null;
        }
    }
}