using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day22
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day22_Test.txt");
            var lines = File.ReadAllLines("Content\\Day22.txt");

            var players = ParseLines(lines);

            var sum = CombatLoop(players);

            Logger.Info($"Day 22A: {sum}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day22_Test.txt");
            //var lines = File.ReadAllLines("Content\\Day22_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day22.txt");

            var players = ParseLines(lines);

            var sum = RecursiveCombatLoop(players);

            Logger.Info($"Day 22B: {sum}");
        }

        private static int CombatLoop(List<Queue<int>> players)
        {
            var player1 = players[0];
            var player2 = players[1];

            int round = 0;
            while (player1.Count != 0 && player2.Count != 0)
            {
                Logger.Debug($"-- Round {round + 1} --");
                Logger.Debug($"Player 1's deck: {string.Join(", ", player1.ToArray())}");
                Logger.Debug($"Player 2's deck: {string.Join(", ", player2.ToArray())}");

                int player1Card = player1.Dequeue();
                int player2Card = player2.Dequeue();

                Logger.Debug($"Player 1 plays: {player1Card}");
                Logger.Debug($"Player 2 plays: {player2Card}");

                if (player1Card > player2Card)
                {
                    Logger.Debug("Player 1 wins the round!");

                    player1.Enqueue(player1Card);
                    player1.Enqueue(player2Card);
                }
                else
                {
                    Logger.Debug("Player 2 wins the round!");

                    player2.Enqueue(player2Card);
                    player2.Enqueue(player1Card);
                }

                round++;

                Logger.Debug(string.Empty);
            }

            Logger.Debug("== Post-game results ==");
            Logger.Debug($"Player 1's deck: {string.Join(", ", player1.ToArray())}");
            Logger.Debug($"Player 2's deck: {string.Join(", ", player2.ToArray())}");

            var winningPlayer = player1.Count > 0 ? player1 : player2;
            var winningCards = winningPlayer.ToList();

            int sum = 0;
            for (var i = 0; i < winningCards.Count; i++)
            {
                sum += winningCards[i] * (winningCards.Count - i);
            }

            return sum;
        }

        private static int RecursiveCombatLoop(List<Queue<int>> players, int game = 0)
        {
            Logger.Debug($"=== Game {game + 1} ===");
            Logger.Debug(string.Empty);

            var player1 = players[0];
            var player2 = players[1];

            int round = 0;

            var player1History = new List<string>();
            var player2History = new List<string>();

            var instantWin = false;

            while (player1.Count != 0 && player2.Count != 0)
            {
                string player1Deck = string.Join(", ", player1.ToArray());
                string player2Deck = string.Join(", ", player2.ToArray());

                if (player1History.Contains(player1Deck) && player2History.Contains(player2Deck))
                {
                    Logger.Debug("Decks the same as before, instant-win for Player 1!");

                    instantWin = true;

                    break;
                }

                player1History.Add(player1Deck);
                player2History.Add(player2Deck);

                Logger.Debug($"-- Round {round + 1} (Game {game + 1}) --");
                Logger.Debug($"Player 1's deck: {player1Deck}");
                Logger.Debug($"Player 2's deck: {player2Deck}");

                int player1Card = player1.Dequeue();
                int player2Card = player2.Dequeue();

                Logger.Debug($"Player 1 plays: {player1Card}");
                Logger.Debug($"Player 2 plays: {player2Card}");

                bool player1Won;

                if (player1.Count >= player1Card && player2.Count >= player2Card)
                {
                    Logger.Debug("Playing a sub-game to determine the winner...");
                    Logger.Debug(string.Empty);

                    var playersCopy = new List<Queue<int>>
                    {
                        new Queue<int>(player1.Take(player1Card)), 
                        new Queue<int>(player2.Take(player2Card))
                    };

                    player1Won = RecursiveCombatLoop(playersCopy, game + 1) == -1;
                }
                else
                {
                    player1Won = player1Card > player2Card;
                }

                if (player1Won)
                {
                    Logger.Debug($"Player 1 wins round {round + 1} of {game + 1}!");

                    player1.Enqueue(player1Card);
                    player1.Enqueue(player2Card);
                }
                else
                {
                    Logger.Debug($"Player 2 wins round {round + 1} of {game + 1}");

                    player2.Enqueue(player2Card);
                    player2.Enqueue(player1Card);
                }

                round++;

                Logger.Debug(string.Empty);
            }

            if (game > 0)
            {
                if (instantWin || player1.Count > player2.Count)
                {
                    Logger.Debug($"The winner of game {game + 1} is player 1!");
                }
                else
                {
                    Logger.Debug($"The winner of game {game + 1} is player 2!");
                }

                Logger.Debug($"...anyway, back to game {game}.");

                return (player1.Count > 0) ? -1 : -2;
            }

            Logger.Debug("== Post-game results ==");
            Logger.Debug($"Player 1's deck: {string.Join(", ", player1.ToArray())}");
            Logger.Debug($"Player 2's deck: {string.Join(", ", player2.ToArray())}");

            var winningPlayer = player1.Count >= player2.Count ? player1 : player2;
            var winningCards = winningPlayer.ToList();

            int sum = 0;
            for (var i = 0; i < winningCards.Count; i++)
            {
                sum += winningCards[i] * (winningCards.Count - i);
            }

            return sum;
        }

        private static List<Queue<int>> ParseLines(string[] lines)
        {
            List<Queue<int>> players = new List<Queue<int>>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("Player"))
                {
                    i++;
                    
                    var queue = new Queue<int>();
                    players.Add(queue);

                    for (; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                        {
                            break;
                        }

                        queue.Enqueue(int.Parse(lines[i]));
                    }
                }
            }

            return players;
        }
    }
}
