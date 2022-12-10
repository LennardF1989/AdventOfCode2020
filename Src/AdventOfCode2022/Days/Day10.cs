using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day10
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day10_Test.txt")
                .ReadAllLines("Content\\Day10.txt")
                ;

            var cycles = 0;
            var x = 1;
            var signalStrength = 0;

            void CheckCycle()
            {
                if (cycles != 20 && (cycles - 20) % 40 != 0)
                {
                    return;
                }

                Logger.Debug($"{cycles} {x}");

                signalStrength += cycles * x;
            }

            foreach (var line in lines)
            {
                var tokens = line.Split(" ");

                if (tokens[0] == "noop")
                {
                    cycles++;

                    CheckCycle();
                }
                else if (tokens[0] == "addx")
                {
                    cycles++;

                    CheckCycle();

                    cycles++;

                    CheckCycle();

                    x += int.Parse(tokens[1]);
                }
            }

            var answer = signalStrength;

            Logger.Info($"Day 10A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day10_Test.txt")
                .ReadAllLines("Content\\Day10.txt")
                ;

            const int screenWidth = 40;

            var cycles = 0;
            var x = 1;

            var currentPosition = 0;
            var currentLine = -1;
            var crtScreen = new List<char>();

            void AddLine()
            {
                for (var i = 0; i < screenWidth; i++)
                {
                    crtScreen.Add('.');
                }
            }

            void UpdateScreen()
            {
                if (currentPosition >= crtScreen.Count)
                {
                    currentLine++;

                    AddLine();
                }

                var position = currentPosition - (currentLine * screenWidth);

                if (position == x - 1 || position == x || position == x + 1)
                {
                    crtScreen[currentPosition] = '#';
                }

                currentPosition++;
            }

            void DrawScreen()
            {
                Logger.Debug($"Cycle {cycles}");
                Logger.Debug(GetScreen());
            }

            string GetScreen()
            {
                var stringBuilder = new StringBuilder();

                for (int i = 0; i < crtScreen.Count; i += screenWidth)
                {
                    if (i != 0)
                    {
                        stringBuilder.AppendLine();
                    }

                    stringBuilder.Append(string.Join("", crtScreen.Skip(i).Take(screenWidth)));
                }

                return stringBuilder.ToString();
            }
            
            foreach (var line in lines)
            {
                var tokens = line.Split(" ");

                if (tokens[0] == "noop")
                {
                    cycles++;

                    UpdateScreen();
                    DrawScreen();
                }
                else if (tokens[0] == "addx")
                {
                    cycles++;

                    UpdateScreen();
                    DrawScreen();

                    cycles++;

                    UpdateScreen();
                    DrawScreen();

                    x += int.Parse(tokens[1]);
                }
            }

            var answer = GetScreen();

            Logger.Info($"Day 10B:\r\n{answer}");
        }
    }
}
