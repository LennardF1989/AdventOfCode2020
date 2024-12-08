namespace AdventOfCode2024.Days
{
    public class Day06 : /*IDayA,*/ IDayB
    {
        private enum Tile
        {
            None = 0,
            Obstacle,
            Guard
        }

        private enum Direction
        {
            Up = 0,
            Right,
            Down,
            Left
        }

        private readonly Dictionary<Direction, (int x, int y)> _directionMap = new()
        {
            { Direction.Up, (0, -1) },
            { Direction.Right, (1, 0) },
            { Direction.Down, (0, 1) },
            { Direction.Left, (-1, 0) }
        };

        public void StartA()
        {
            var (guardPosition, grid) = ParseInput();

            var newGuardPosition = ((int, int)?)guardPosition;
            var guardDirection = Direction.Up;
            var visitedTiles = new HashSet<(Direction direction, (int x, int y) position)>
            {
                (guardDirection, newGuardPosition.Value)
            };
            
            do
            {
                newGuardPosition = Move(
                    grid, 
                    newGuardPosition.Value, 
                    guardDirection,
                    _directionMap[guardDirection],
                    visitedTiles
                );

                guardDirection = (Direction)(((int)guardDirection + 1) % 4);
            } while (newGuardPosition != null);

            var answer = visitedTiles
                .SelectHashSet(x => x.Item2)
                .Count;

            Logger.Info($"Day 6A: {answer}");
        }

        public void StartB()
        {
            var (guardPosition, grid) = ParseInput();

            HashSet<(Direction direction, (int x, int y) position)> visitedTiles;
            
            //First simulation
            {
                var newGuardPosition = ((int, int)?)guardPosition;
                var guardDirection = Direction.Up;
                visitedTiles = [(guardDirection, newGuardPosition.Value)];

                do
                {
                    newGuardPosition = Move(
                        grid, 
                        newGuardPosition.Value, 
                        guardDirection,
                        _directionMap[guardDirection],
                        visitedTiles
                    );
                    
                    guardDirection = (Direction)(((int)guardDirection + 1) % 4);
                } while (newGuardPosition != null);
            }

            //Remove the starting position as a potential obstacle position
            visitedTiles.Remove((Direction.Up, guardPosition));

            var possiblePositions = new HashSet<(int x, int y)>();
            
            foreach (var tile in visitedTiles)
            {
                grid[tile.position.y][tile.position.x] = Tile.Obstacle;

                var newGuardPosition = ((int, int)?)guardPosition;
                var guardDirection = Direction.Up;
                var newVisitedTiles = new HashSet<(Direction, (int x, int y))>
                {
                    (guardDirection, newGuardPosition.Value)
                };
                
                do
                {
                    newGuardPosition = Move(
                        grid, 
                        newGuardPosition.Value,
                        guardDirection,
                        _directionMap[guardDirection],
                        newVisitedTiles
                    );
                    
                    guardDirection = (Direction)(((int)guardDirection + 1) % 4);
                } while (newGuardPosition != null && newGuardPosition != (-1, -1));

                if (newGuardPosition == (-1, -1))
                {
                    possiblePositions.Add(tile.position);
                }
                
                grid[tile.position.y][tile.position.x] = Tile.None;
            }

            var answer = possiblePositions.Count;

            Logger.Info($"Day 6B: {answer}");
        }

        private static (int, int)? Move(
            List<List<Tile>> grid,
            (int x, int y) guardPosition,
            Direction guardDirection,
            (int x, int y) moveDirection,
            HashSet<(Direction direction, (int x, int y) position)> visitedTiles
        )
        {
            while (true)
            {
                var newX = guardPosition.x + moveDirection.x;
                var newY = guardPosition.y + moveDirection.y;

                if (
                    newX < 0 ||
                    newX >= grid[0].Count ||
                    newY < 0 ||
                    newY >= grid.Count
                )
                {
                    return null;
                }

                var tile = grid[newY][newX];

                if (tile != Tile.Obstacle)
                {
                    guardPosition = (newX, newY);

                    // NOTE: Infinite loop!
                    if (!visitedTiles.Add((guardDirection, guardPosition)))
                    {
                        return (-1, -1);
                    }
                }
                else
                {
                    return guardPosition;
                }
            }
        }

        private static (
            (int, int) guardPosition,
            List<List<Tile>> grid
            ) ParseInput()
        {
            var guardPosition = (0, 0);

            var grid = File
                //.ReadAllLines("Content\\Day06_Test.txt")
                .ReadAllLines("Content\\Day06.txt")
                .Select((x, posY) => x
                    .Select((y, posX) =>
                    {
                        var result = y switch
                        {
                            '#' => Tile.Obstacle,
                            '^' => Tile.Guard,
                            _ => Tile.None
                        };

                        if (result != Tile.Guard)
                        {
                            return result;
                        }

                        guardPosition = (posX, posY);

                        return Tile.None;
                    })
                    .ToList()
                )
                .ToList();

            return (guardPosition, grid);
        }
    }
}