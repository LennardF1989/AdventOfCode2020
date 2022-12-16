using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using static AdventOfCode2022.Days.Day16;

namespace AdventOfCode2022.Days
{
    public static class Day16
    {
        public class Valve
        {
            public string Name { get; set; }
            public int FlowRate { get; set; }
            public List<Valve> Tunnels { get; set; }

            public override string ToString()
            {
                return $"{Name} ({FlowRate})";
            }
        }

        public static void StartA()
        {
            var lines = File
                .ReadAllLines("Content\\Day16_Test.txt")
                //.ReadAllLines("Content\\Day16.txt")
                .Select(x =>
                {
                    var match = Regex.Match(x, "Valve (.*?) has flow rate=(.*?); tunnel(s){0,1} lead(s){0,1} to valve(s){0,1} (.*)");

                    return (
                        valve: new Valve
                        {
                            Name = match.Groups[1].Value,
                            FlowRate = int.Parse(match.Groups[2].Value),
                            Tunnels = new List<Valve>()
                        }, 
                        tunnels: match.Groups[6].Value.Split(",").Select(y => y.Trim()).ToList()
                    );
                })
                .ToList()
                ;

            var tunnelLookup = lines.ToDictionary(x => x.valve.Name);

            lines.ForEach(x =>
            {
                foreach (var tunnel in tunnelLookup[x.valve.Name].tunnels)
                {
                    x.valve.Tunnels.Add(tunnelLookup[tunnel].valve);
                }
            });

            var paths = new Dictionary<Valve, Dictionary<Valve, int>>();
            foreach (var line in lines)
            {
                var result = ALen(line.valve);

                paths.Add(line.valve, result);
            }

            var answer = 0;

            Logger.Info($"Day 16A: {answer}");
        }

        //NOTE: But probably more like Dijkstra?
        private static Dictionary<Valve, int> ALen(Valve start)
        {
            var openSet = new Queue<Valve>();

            var gScore = new Dictionary<Valve, int>
            {
                [start] = 0
            };

            var closedSet = new HashSet<Valve>();

            openSet.Enqueue(start);

            while (openSet.Count > 0)
            {
                var valve = openSet.Dequeue();

                foreach (var tunnel in valve.Tunnels)
                {
                    if (closedSet.Contains(tunnel))
                    {
                        continue;
                    }

                    closedSet.Add(tunnel);

                    gScore[tunnel] = gScore[valve] + 1;

                    openSet.Enqueue(tunnel);
                }
            }

            return gScore;
        }

        public static void StartB()
        {
            var lines = File
                .ReadAllLines("Content\\Day16_Test.txt")
                //.ReadAllLines("Content\\Day16.txt")
                ;

            var answer = 0;

            Logger.Info($"Day 16B: {answer}");
        }

        private static List<Valve> AStar(Valve start, int highestScore)
        {
            var openSet = new PriorityQueue<(Valve valve, List<Valve> path, bool open, int minute), int>();
            var parentMap = new Dictionary<Valve, Valve>();

            var gScore = new Dictionary<Valve, int>
            {
                [start] = 0
            };

            openSet.Enqueue((start, new List<Valve>(), false, 1), 0);

            while (openSet.Count > 0)
            {
                var s = openSet.Dequeue();

                if (s.minute >= 30)
                {
                    return s.path;
                }
                
                var node = s.valve;

                var list = new List<Valve>(node.Tunnels);

                if (s.open)
                {

                }

                foreach (var neighbor in list)
                {
                    var cost = gScore[node] + 1;

                    parentMap[neighbor] = node;
                    gScore[neighbor] = cost;

                    var fScore = highestScore - neighbor.FlowRate;

                    var path = new List<Valve>(s.path) { node };

                    openSet.Enqueue((neighbor, path, false, s.minute + 1), cost + fScore);
                }
            }

            return null;
        }
    }
}
