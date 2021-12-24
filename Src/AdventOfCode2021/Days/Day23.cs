using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day23
    {
        public enum State : byte
        {
            Amber,
            Bronze,
            Copper,
            Desert,
            Empty,
            Forbidden
        }

        public class Level
        {
            public int Cost { get; set; }
            public State[] Corridor { get; set; }
            public Stack<State>[] Rooms { get; set; }

            public Level Clone()
            {
                return new Level
                {
                    Cost = Cost,
                    Corridor = Corridor.ToArray(),
                    Rooms = Rooms.Select(x => new Stack<State>(x.Reverse().ToArray())).ToArray()
                };
            }

            public string Id()
            {
                var corridor = string.Join(string.Empty, Corridor.Select(x => (int)x));
                var room = string.Join("_", Rooms.Select(x =>
                    string.Join(string.Empty, x.Select(y => (int)y)).PadLeft(4, '.')
                ));

                return $"{corridor}_{room}";
            }
        }

        private static readonly int[] _cost =
        {
            1,
            10,
            100,
            1000
        };

        public static void StartA()
        {
            var level = new Level
            {
                Cost = 0,
                Corridor = Enumerable.Repeat(State.Empty, 11).ToArray(),
                Rooms = new[]
                {
                    new Stack<State>(new [] { State.Desert, State.Desert }),
                    new Stack<State>(new [] { State.Copper, State.Amber }),
                    new Stack<State>(new [] { State.Bronze, State.Copper }),
                    new Stack<State>(new [] { State.Bronze, State.Amber })
                }
            };

            level.Corridor[2] = level.Corridor[4] = level.Corridor[6] = level.Corridor[8] = State.Forbidden;

            var answer = SolveLevel(level, 2);

            Logger.Info($"Day 23A: {answer}");
        }

        public static void StartB()
        {
            var level = new Level
            {
                Cost = 0,
                Corridor = Enumerable.Repeat(State.Empty, 11).ToArray(),
                Rooms = new[]
                {
                    new Stack<State>(new []
                    {
                        State.Desert, State.Desert, State.Desert, State.Desert
                    }),
                    new Stack<State>(new []
                    {
                        State.Copper, State.Bronze, State.Copper, State.Amber
                    }),
                    new Stack<State>(new []
                    {
                        State.Bronze, State.Amber, State.Bronze, State.Copper
                    }),
                    new Stack<State>(new []
                    {
                        State.Bronze, State.Copper, State.Amber, State.Amber
                    })
                }
            };

            level.Corridor[2] = level.Corridor[4] = level.Corridor[6] = level.Corridor[8] = State.Forbidden;

            var answer = SolveLevel(level, 4);

            Logger.Info($"Day 23B: {answer}");
        }

        private static int SolveLevel(Level firstLevel, int roomSize)
        {
            //NOTE: Has to be a priority queue! Otherwise the visited hashset will exclude more optimal solutions...
            var clones = new PriorityQueue<Level, int>();
            clones.Enqueue(firstLevel, firstLevel.Cost);

            var visited = new HashSet<string>();

            while (clones.Count > 0)
            {
                var level = clones.Dequeue();

                var id = level.Id();

                if (visited.Contains(id))
                {
                    continue;
                }

                visited.Add(id);

                //Did we win?
                if (
                    level.Rooms[0].Count == roomSize && level.Rooms[0].All(x => x == State.Amber) &&
                    level.Rooms[1].Count == roomSize && level.Rooms[1].All(x => x == State.Bronze) &&
                    level.Rooms[2].Count == roomSize && level.Rooms[2].All(x => x == State.Copper) &&
                    level.Rooms[3].Count == roomSize && level.Rooms[3].All(x => x == State.Desert)
                )
                {
                    return level.Cost;
                }

                for (var c = 0; c < level.Corridor.Length; c++)
                {
                    var corridor = level.Corridor[c];

                    if (corridor >= State.Empty)
                    {
                        continue;
                    }

                    var possibleRoomIndex = (int)corridor;
                    var possibleRoom = level.Rooms[possibleRoomIndex];

                    if (possibleRoom.Any(x => x != corridor))
                    {
                        continue;
                    }

                    var beginIndex = c;
                    var targetIndex = (possibleRoomIndex + 1) * 2;

                    if (beginIndex < targetIndex)
                    {
                        for (var cc = beginIndex + 1; cc < targetIndex; cc++)
                        {
                            var currentCorridor = level.Corridor[cc];

                            if (currentCorridor < State.Empty)
                            {
                                goto reset;
                            }
                        }
                    }
                    else if (beginIndex > targetIndex)
                    {
                        for (var cc = beginIndex - 1; cc > targetIndex; cc--)
                        {
                            var currentCorridor = level.Corridor[cc];

                            if (currentCorridor < State.Empty)
                            {
                                goto reset;
                            }
                        }
                    }

                    var clone = level.Clone();

                    var cost = ((roomSize - clone.Rooms[possibleRoomIndex].Count) + Math.Abs(beginIndex - targetIndex)) * _cost[(int)corridor];

                    clone.Cost += cost;
                    clone.Corridor[c] = State.Empty;
                    clone.Rooms[possibleRoomIndex].Push(corridor);
                    clones.Enqueue(clone, clone.Cost);

                    reset:;
                }

                for (var s = 0; s < level.Rooms.Length; s++)
                {
                    var room = level.Rooms[s];
                    var amphipod = room.Any()
                        ? room.Peek()
                        : State.Empty;

                    if (amphipod == State.Empty)
                    {
                        continue;
                    }

                    var corridorIndex = s * 2 + 2;
                    State corridor;

                    //Determine all possible moves moving to left
                    var leftCorridor = corridorIndex - 1;
                    do
                    {
                        corridor = level.Corridor[leftCorridor];

                        if (corridor == State.Empty)
                        {
                            var clone = level.Clone();

                            var cost = ((roomSize + 1 - clone.Rooms[s].Count) + (corridorIndex - leftCorridor)) * _cost[(int)amphipod];

                            clone.Cost += cost;
                            clone.Rooms[s].Pop();
                            clone.Corridor[leftCorridor] = amphipod;
                            clones.Enqueue(clone, clone.Cost);
                        }

                        leftCorridor--;
                    } while (leftCorridor >= 0 && corridor >= State.Empty);

                    //Determine all possible moves moving to right
                    var rightCorridor = corridorIndex + 1;
                    do
                    {
                        corridor = level.Corridor[rightCorridor];

                        if (corridor == State.Empty)
                        {
                            var clone = level.Clone();

                            var cost = ((roomSize + 1 - clone.Rooms[s].Count) + (rightCorridor - corridorIndex)) * _cost[(int)amphipod];

                            clone.Cost += cost;
                            clone.Rooms[s].Pop();
                            clone.Corridor[rightCorridor] = amphipod;
                            clones.Enqueue(clone, clone.Cost);
                        }

                        rightCorridor++;
                    } while (rightCorridor < level.Corridor.Length && corridor >= State.Empty);
                }
            }

            return 0;
        }
    }
}
