using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day03
    {
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt");
                .ReadAllLines("Content\\Day03.txt");

            StringBuilder gammaRate = new StringBuilder();
            StringBuilder epsilonRate = new StringBuilder();

            for (var offset = 0; offset < lines[0].Length; offset++)
            {
                var amountZero = 0;
                var amountOne = 0;

                foreach (var line in lines)
                {
                    if (line[offset] == '0')
                    {
                        amountZero++;
                    }
                    else
                    {
                        amountOne++;
                    }
                }

                gammaRate.Append((amountOne > amountZero) ? "1" : "0");
                epsilonRate.Append((amountOne < amountZero) ? "1" : "0");
            }

            int gammaRateInt = Convert.ToInt32(gammaRate.ToString(), 2);
            int epsiloneRateInt = Convert.ToInt32(epsilonRate.ToString(), 2);

            Logger.Info($"Day 3A: {gammaRateInt * epsiloneRateInt}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt")
                .ReadAllLines("Content\\Day03.txt")
                .ToList();

            string gammaRate = "";
            string epsilonRate = "";

            List<string> loopValues = lines;
            int offset = 0;

            while(loopValues.Count > 1 && offset < lines[0].Length)
            {
                List<string> amountZeros = loopValues.Where(x => x[offset] == '0').ToList();
                List<string> amountOnes = loopValues.Where(x => x[offset] == '1').ToList();

                loopValues = amountOnes.Count >= amountZeros.Count ? amountOnes : amountZeros;
                offset++;
            }

            int oxygenRate = Convert.ToInt32(loopValues[0], 2);

            loopValues = lines;
            offset = 0;

            while(loopValues.Count > 1 && offset < lines[0].Length)
            {
                List<string> amountZeros = loopValues.Where(x => x[offset] == '0').ToList();
                List<string> amountOnes = loopValues.Where(x => x[offset] == '1').ToList();

                loopValues = amountZeros.Count <= amountOnes.Count ? amountZeros : amountOnes;
                offset++;
            }

            int scrubberRate = Convert.ToInt32(loopValues[0], 2);

            Logger.Info($"Day 3B: {oxygenRate * scrubberRate}");
        }
    }
}
