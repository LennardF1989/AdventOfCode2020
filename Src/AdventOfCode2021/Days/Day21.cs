using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day21
    {
        public static void StartA()
        {
            var playerPositions = File
                .ReadAllLines("Content\\Day21.txt")
                //.ReadAllLines("Content\\Day21_Test.txt")
                .Select(x => int.Parse(x.Split(":")[1]))
                .ToArray();

            int[] playerScores = new int[2];

            int dice = 0;

            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    var moves = (dice * 3) + 6;
                    dice += 3;

                    if (MovePlayer(moves, playerPositions, playerScores, i, 1000))
                    {
                        goto done;
                    }
                }
            }

        done:
            int answer = playerScores.Min() * dice;

            Logger.Info($"Day 21A: {answer}");
        }

        //Disclaimer: I had to get some hints for this, I just couldn't see what I was doing wrong spawning universes...
        public static void StartB()
        {
            var playerPositions = File
                .ReadAllLines("Content\\Day21.txt")
                //.ReadAllLines("Content\\Day21_Test.txt")
                .Select(x => int.Parse(x.Split(":")[1]))
                .ToArray();

            var result = GetWins(playerPositions);

            var answer = result.Max();

            Logger.Info($"Day 21B: {answer}");
        }

        private static bool MovePlayer(int moves, int[] playerPositions, int[] playerScores, int player, int maxScore)
        {
            /*int currentPosition = playerPositions[player];

            for (var i = 0; i < moves; i++)
            {
                currentPosition++;

                if (currentPosition == 11)
                {
                    currentPosition = 1;
                }
            }*/

            int remainder = (playerPositions[player] + moves) % 10;
            remainder = remainder == 0 ? 10 : remainder;

            playerPositions[player] = remainder;
            playerScores[player] += remainder;

            return playerScores[player] >= maxScore;
        }

        private static long[] GetWins(int[] startPositions)
        {
            int[] possibleCombinations = new int[27];

            for (var i = 1; i <= 3; i++)
            {
                for (var i2 = 1; i2 <= 3; i2++)
                {
                    for (var i3 = 1; i3 <= 3; i3++)
                    {
                        possibleCombinations[(i - 1) * 3 * 3 + (i2 - 1) * 3 + (i3 - 1)] = i + i2 + i3;
                    }
                }
            }

            var visited = new Dictionary<string, long[]>();

            long[] GetWinsRecursive(int[] playerPositions, int[] playerScores, int currentPlayer)
            {
                if (visited.TryGetValue(
                    $"{playerPositions[0]}_{playerPositions[1]}_{playerScores[0]}_{playerScores[1]}_{currentPlayer}",
                    out var actualRound)
                )
                {
                    return actualRound;
                }

                long[] winCount = new long[2];

                foreach (var moves in possibleCombinations)
                {
                    int[] positions = { playerPositions[0], playerPositions[1] };
                    int[] scores = { playerScores[0], playerScores[1] };

                    long[] result;

                    if (MovePlayer(moves, positions, scores, currentPlayer, 21))
                    {
                        result = currentPlayer == 0 ? new long[] { 1, 0 } : new long[] { 0, 1 };
                    }
                    else
                    {
                        result = GetWinsRecursive(positions, scores, currentPlayer == 0 ? 1 : 0);
                    }

                    winCount[0] += result[0];
                    winCount[1] += result[1];
                }

                visited[$"{playerPositions[0]}_{playerPositions[1]}_{playerScores[0]}_{playerScores[1]}_{currentPlayer}"] = winCount;

                return winCount;
            }

            return GetWinsRecursive(startPositions, new[] { 0, 0 }, 0);
        }
    }
}
