namespace AdventOfCode2024.Days
{
    public class Day07 : IDayA, IDayB
    {
        public void StartA()
        {
            var lines = ParseInput();

            var sum = 0L;
            
            foreach (var line in lines)
            {
                var possibilities = RunEquation(
                    line.answer, 
                    line.values[0], 
                    1, 
                    line.values, 
                    false
                );

                if (possibilities > 0)
                {
                    sum += line.answer;
                }
            }

            var answer = sum;

            Logger.Info($"Day 7A: {answer}");
        }
        
        public void StartB()
        {
            var lines = ParseInput();
            
            var tryAgainLines = new List<(long answer, List<long> values)>();
            var sum = 0L;
            
            //Round 1
            foreach (var line in lines)
            {
                var possibilities = RunEquation(
                    line.answer, 
                    line.values[0], 
                    1, 
                    line.values, 
                    false
                );

                if (possibilities > 0)
                {
                    sum += line.answer;
                }
                else
                {
                    tryAgainLines.Add(line);
                }
            }
            
            //Round 2
            foreach (var line in tryAgainLines)
            {
                var possibilities = RunEquation(line.answer, line.values[0], 1, line.values, true);

                if (possibilities > 0)
                {
                    sum += line.answer;
                }
            }

            var answer = sum;

            Logger.Info($"Day 7B: {answer}");
        }
        
        private static int RunEquation(
            long answer, 
            long currentAnswer, 
            int index, 
            List<long> values,
            bool concatenate
        )
        {
            if (currentAnswer > answer)
            {
                return 0;
            }

            if (index >= values.Count)
            {
                return currentAnswer == answer ? 1 : 0;
            }
            
            var result = 0;
            
            var a = values[index];

            if (concatenate)
            {
                result += RunEquation(
                    answer, 
                    (long)(currentAnswer * Math.Pow(10, (long)Math.Log10(a) + 1)) + a,
                    index + 1, 
                    values,
                    true
                );   
            }
            
            result += RunEquation(answer, currentAnswer * a, index + 1, values, concatenate);
            result += RunEquation(answer, currentAnswer + a, index + 1, values, concatenate);

            return result;    
        }
        
        private static List<(long answer, List<long> values)> ParseInput()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                .SelectList(x =>
                {
                    var split = x.Split(":", true, true);

                    var a = split[0].ToLong();
                    var b = split[1].Split(" ").SelectList(y => y.ToLong());

                    return (answer: a, values: b);
                });
            
            return lines;
        }
    }
}