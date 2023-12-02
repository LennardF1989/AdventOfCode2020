using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Extensions;

namespace AdventOfCode2023.Days
{
    public static class Day02
    {
        public static void StartA()
        {
            var games = ParseLines();

            var answer = 0;

            foreach (var (gameId, rounds) in games)
            {
                var valid = true;

                foreach (var round in rounds.TakeWhile(round => valid))
                {
                    foreach (var (amount, color) in round)
                    {
                        if (
                            (color == "red" && amount <= 12) ||
                            (color == "green" && amount <= 13) ||
                            (color == "blue" && amount <= 14)
                        )
                        {
                            continue;
                        }

                        valid = false;

                        break;
                    }
                }

                if (valid)
                {
                    answer += gameId;
                }
            }

            Logger.Info($"Day 2A: {answer}");
        }

        public static void StartB()
        {
            var games = ParseLines();

            var answer = 0;

            foreach (var (_, rounds) in games)
            {
                var red = 0;
                var green = 0;
                var blue = 0;

                foreach (var round in rounds)
                {
                    foreach (var (amount, color) in round)
                    {
                        switch (color)
                        {
                            case "red":
                                red = Math.Max(red, amount);
                                break;
                            case "green":
                                green = Math.Max(green, amount);
                                break;
                            case "blue":
                                blue = Math.Max(blue, amount);
                                break;
                        }
                    }
                }

                var power = red * green * blue;
                answer += power;
            }

            Logger.Info($"Day 2B: {answer}");
        }

        private static List<(int gameId, List<List<(int amount, string color)>> rounds)> ParseLines()
        {
            return File
                //.ReadAllLines("Content\\Day02_Test.txt")
                .ReadAllLines("Content\\Day02.txt")
                .SelectList(x =>
                {
                    var game = x.Split(":");
                    var gameId = game[0].Split(" ")[^1].ToInteger();
                    var rounds = game[1]
                        .Split(";")
                        .SelectList(y => y
                            .Split(",", true)
                            .SelectList(z =>
                            {
                                var cubes = z.Split(" ");

                                return (
                                    amount: cubes[0].ToInteger(),
                                    color: cubes[1]
                                );
                            })
                        );

                    return (gameId, rounds);
                });
        }
    }
}
