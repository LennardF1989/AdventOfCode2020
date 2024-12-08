namespace AdventOfCode2024.Days
{
    public class Day02 : IDayA, IDayB
    {
        private enum Movement
        {
            None = 0,
            Increase,
            Decrease
        }
        
        public void StartA()
        {
            var levels = ParseInput();

            var safeLevels = 0;
            
            foreach (var level in levels)
            {
                var safeLevel = true;
                var movement = level[0] - level[1] < 0; //true means increase
                
                for (var i = 0; i < level.Count - 1; i++)
                {
                    var a = level[i];
                    var b = level[i + 1];
                    
                    var difference = a - b;

                    if (
                        (movement && difference > 0) ||
                        (!movement && difference < 0) ||
                        Math.Abs(difference) is < 1 or > 3
                    )
                    {
                        safeLevel = false;
                        break;  
                    }
                }

                if (safeLevel)
                {
                    safeLevels++;
                }
            }
            
            var answer = safeLevels;

            Logger.Info($"Day 2A: {answer}");
        }
        
        public void StartB()
        {
            var levels = ParseInput();

            var safeLevels = 0;
            
            foreach (var level in levels)
            {
                var result = IsLevelSafe(level);

                if (!result.safe)
                {
                    // Try modifying the report by removing 1 around the detected index
                    for (var i = result.index - 1; i <= result.index + 1; i++)
                    {
                        if (i < 0 || i >= level.Count)
                        {
                            continue;
                        }
                        
                        var copy = level.ToList();
                        copy.RemoveAt(i);

                        var resultCopy = IsLevelSafe(copy);

                        if (!resultCopy.Item1)
                        {
                            continue;
                        }
                        
                        safeLevels++;
                        break;
                    }
                }
                else
                {
                    safeLevels++;
                }
            }
            
            var answer = safeLevels;

            Logger.Info($"Day 2B: {answer}");
        }

        private static (bool safe, int index) IsLevelSafe(List<int> level)
        {
            var movement = Movement.None;
            
            for (var i = 0; i < level.Count - 1; i++)
            {
                var a = level[i];
                var b = level[i + 1];
                    
                var difference = a - b;

                // Make sure the difference is actually a valid value
                if (Math.Abs(difference) is < 1 or > 3)
                {
                    return (false, i);  
                }

                if (movement == Movement.None)
                {
                    movement = difference < 0 
                        ? Movement.Increase 
                        : Movement.Decrease;
                }
                    
                if ((movement == Movement.Increase && difference > 0) ||
                    (movement == Movement.Decrease && difference < 0))
                {
                    return (false, i);
                }
            }

            return (true, 0);
        }
        
        private static List<List<int>> ParseInput()
        {
            return File
                //.ReadAllLines("Content\\Day02_Test.txt")
                .ReadAllLines("Content\\Day02.txt")
                .SelectList(x =>
                {
                    return x
                        .Split(" ", true, true)
                        .SelectList(y => y.ToInteger());
                });
        }
    }
}
