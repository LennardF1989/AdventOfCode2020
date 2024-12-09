namespace AdventOfCode2024.Days
{
    public class Day08 : IDayA, IDayB
    {
        public void StartA()
        {
            var input = ParseInput();
            var antinodes = new HashSet<(int x, int y)>();
            
            foreach (var kvp in input.antennas)
            {
                for (var i = 0; i < kvp.Value.Count; i++)
                {
                    var a1 = kvp.Value[i];

                    for (var i2 = 0; i2 < kvp.Value.Count; i2++)
                    {
                        if (i == i2)
                        {
                            continue;
                        }
                        
                        var a2 = kvp.Value[i2];

                        var diffX = a1.x - a2.x;
                        var diffY = a1.y - a2.y;

                        var antinode = (a1.x + diffX, a1.y + diffY);

                        antinodes.Add(antinode);
                    }
                }
            }

            var answer = antinodes.Count(a =>
                a.x >= 0 &&
                a.x < input.width &&
                a.y >= 0 &&
                a.y < input.height
            );

            Logger.Info($"Day 8A: {answer}");
        }
        
        public void StartB()
        {
            var input = ParseInput();
            var antinodes = new HashSet<(int x, int y)>();
            
            foreach (var kvp in input.antennas)
            {
                for (var i = 0; i < kvp.Value.Count; i++)
                {
                    var a1 = kvp.Value[i];

                    antinodes.Add(a1);
                    
                    for (var i2 = 0; i2 < kvp.Value.Count; i2++)
                    {
                        if (i == i2)
                        {
                            continue;
                        }
                        
                        var a2 = kvp.Value[i2];

                        var diffX = a1.x - a2.x;
                        var diffY = a1.y - a2.y;

                        var newX = a1.x + diffX;
                        var newY = a1.y + diffY;
                        
                        while (newX >= 0 && newX < input.width && newY >= 0 && newY < input.height)
                        {
                            var antinode = (x: newX, y: newY);
                            
                            antinodes.Add(antinode);
                            
                            newX = antinode.x + diffX;
                            newY = antinode.y + diffY;
                        }
                    }
                }
            }

            var answer = antinodes.Count;

            Logger.Info($"Day 8B: {answer}");
        }


        private static (int width, int height, Dictionary<char, List<(int x, int y)>> antennas) ParseInput()
        {
            var grid = File
                //.ReadAllLines("Content\\Day08_Test.txt")
                .ReadAllLines("Content\\Day08.txt")
                .SelectList(x => x.ToList())
                ;

            var antennas = new Dictionary<char, List<(int x, int y)>>(); 

            for (var y = 0; y < grid.Count; y++)
            {
                for (var x = 0; x < grid[y].Count; x++)
                {
                    if (grid[y][x] == '.')
                    {
                        continue;
                    }
                    
                    if (!antennas.TryGetValue(grid[y][x], out var list))
                    {
                        list = antennas[grid[y][x]] = new List<(int x, int y)>();
                    }
                        
                    list.Add((x, y));
                }
            }

            return (grid[0].Count, grid.Count, antennas);
        }
    }
}
