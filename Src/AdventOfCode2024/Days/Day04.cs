namespace AdventOfCode2024.Days
{
    public class Day04 : IDayA, IDayB
    {
        private static readonly List<(int x, int y)> _surroundingCoords1 =
        [
            (-1, -1),
            (0, -1),
            (1, -1),
            (-1, 0),
            (1, 0),
            (-1, 1),
            (0, 1),
            (1, 1)
        ];
        
        private static readonly List<(int x, int y)> _surroundingCoords2 =
        [
            (-1, -1),
            (1, -1),
            (-1, 1),
            (1, 1)
        ];
        
        public void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day04_Test.txt")
                .ReadAllLines("Content\\Day04.txt")
                .SelectList(x => x.ToList());

            var total = 0;
            
            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Count; x++)
                {
                    if (lines[y][x] != 'X')
                    {
                        continue;
                    }

                    foreach (var surroundingCoord in _surroundingCoords1)
                    {
                        var s = "X";
                        
                        for (var i = 1; i < 4; i++)
                        {
                            var newX = x + surroundingCoord.x * i;
                            var newY = y + surroundingCoord.y * i;
                            
                            if (newX < 0 || 
                                newX >= lines[0].Count ||
                                newY < 0 ||
                                newY >= lines.Count)
                            {
                                break;
                            }
                            
                            s += lines[newY][newX];
                        }

                        if (s == "XMAS")
                        {
                            total++;
                        }
                    }
                }
            }

            var answer = total;

            Logger.Info($"Day 4A: {answer}");
        }

        public void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day04_Test.txt")
                .ReadAllLines("Content\\Day04.txt")
                .SelectList(x => x.ToList());

            var total = 0;
            
            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Count; x++)
                {
                    if (lines[y][x] != 'A')
                    {
                        continue;
                    }

                    var s = "";
                    
                    foreach (var surroundingCoord in _surroundingCoords2)
                    {
                        var newX = x + surroundingCoord.x;
                        var newY = y + surroundingCoord.y;
                            
                        if (newX < 0 || 
                            newX >= lines[0].Count ||
                            newY < 0 ||
                            newY >= lines.Count)
                        {
                            break;
                        }
                        
                        s += lines[newY][newX];
                    }

                    if (s is "MSMS" or "SMSM" or "SSMM" or "MMSS")
                    {
                        total++;
                    }
                }
            }

            var answer = total;

            Logger.Info($"Day 4B: {answer}");
        }
    }
}
