using System.Text.RegularExpressions;

namespace AdventOfCode2024.Days
{
    public class Day03 : IDayA, IDayB
    {
        public void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test.txt")
                .ReadAllLines("Content\\Day03.txt")
                .ToList();

            var total = 0L;
            
            foreach (var line in lines)
            {
                var multipliers = Regex.Matches(line, "mul\\((\\d+),(\\d+)\\)");

                total += multipliers.Sum(
                    x => x.Groups[1].Value.ToLong() * x.Groups[2].Value.ToLong()
                );
            }
            
            var answer = total;

            Logger.Info($"Day 3A: {answer}");
        }
        
        public void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day03_Test2.txt")
                .ReadAllLines("Content\\Day03.txt")
                .ToList();

            var total = 0L;

            var doInstruction = true;
            
            foreach (var line in lines)
            {
                var multipliers = Regex.Matches(
                    line, 
                    @"do\(\)|don\'t\(\)|mul\((\d+),(\d+)\)"
                );
                
                foreach (Match match in multipliers)
                {
                    if (match.Value == "do()")
                    {
                        doInstruction = true;
                    }
                    else if (match.Value == "don't()")
                    {
                        doInstruction = false;
                    }
                    else if(doInstruction)
                    {
                        total += match.Groups[1].Value.ToLong() * match.Groups[2].Value.ToLong();
                    }
                }
            }

            var answer = total;

            Logger.Info($"Day 3B: {answer}");
        }
    }
}
