using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using static System.Formats.Asn1.AsnWriter;

namespace AdventOfCode2022.Days
{
    //TODO: Learn DP with bitmask trick
    //DONE: Learn Floyd–Warshall!
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
                //.ReadAllLines("Content//Day16_Test.txt")
                .ReadAllLines("Content//Day16.txt")
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

            var start = lines.First(x => x.valve.Name == "AA").valve;

            //Do a BFS
            var queue = new Queue<(int time, Valve location, int score, HashSet<Valve> opened)>();
            queue.Enqueue((1, start, 0, new HashSet<Valve>()));

            var visited = new Dictionary<(int time, Valve location), int>();
            var best = 0;
            
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                
                if (visited.GetValueOrDefault((v.time, v.location), -1) >= v.score)
                {
                    continue;
                }
                
                visited[(v.time, v.location)] = v.score;

                if (v.time == 30)
                {
                    best = Math.Max(best, v.score);
                    
                    continue;
                }
                
                //Open valve
                if (v.location.FlowRate > 0 && !v.opened.Contains(v.location))
                {
                    var opened = new HashSet<Valve>(v.opened)
                    {
                        v.location
                    };

                    var score = v.score + opened.Sum(x => x.FlowRate);
                    var state = (v.time + 1, v.location, score, opened);
                    queue.Enqueue(state);
                }
                
                //Go somewhere
                {
                    var score = v.score + v.opened.Sum(x => x.FlowRate);

                    foreach (var tunnel in v.location.Tunnels)
                    {
                        var opened = new HashSet<Valve>(v.opened);
                        var state = (v.time + 1, tunnel, score, opened);
                        queue.Enqueue(state);
                    }
                }
            }

            var answer = best;
            
            Logger.Info($"Day 16A: {answer}");
        }
        
        public static void StartB()
        {
            var lines = File
                    //.ReadAllLines("Content//Day16_Test.txt")
                    .ReadAllLines("Content//Day16.txt")
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

            //NOTE: Couldn't we also just have optimized the graph by making weighted edges and replace the zero
            //nodes with a higher cost?

            //All values
            var allValves = lines.Select(x => x.valve).ToList();
            var allUsefulValves = lines.Select(x => x.valve).Where(x => x.FlowRate > 0).ToList();

            //Floyd-Warshall
            var floydWarshall = FloydWarshall(allValves);
            var floydWarshallDictionary = FloydWarshallAsDictionary(allValves, floydWarshall);

            var start = tunnelLookup["AA"].valve;
            var answer = 0;

            //Then... just bruteforce the hell out of it!
            {
                IEnumerable<HashSet<Valve>> GenerateOpenOptions(Valve current, HashSet<Valve> openValves, int timeLeft)
                {
                    foreach (var next in allUsefulValves)
                    {
                        if (!openValves.Contains(next) && floydWarshallDictionary[current][next] < timeLeft)
                        {
                            openValves.Add(next);

                            var result = GenerateOpenOptions(
                                next, openValves, timeLeft - floydWarshallDictionary[current][next] - 1
                            );

                            foreach (var r in result)
                            {
                                yield return r;
                            }

                            openValves.Remove(openValves.Last());
                        }
                    }

                    yield return new HashSet<Valve>(openValves);
                }

                int GetOrderScore(HashSet<Valve> openValves, int timeLeft)
                {
                    var current = start;
                    var score = 0;

                    foreach (var valve in openValves)
                    {
                        timeLeft -= floydWarshallDictionary[current][valve] + 1;
                        score += valve.FlowRate * timeLeft;
                        current = valve;
                    }

                    return score;
                }

                var ways = GenerateOpenOptions(start, new HashSet<Valve>(), 26).ToList();

                var bestScoresDictionary = new Dictionary<string, (HashSet<Valve>, int)> ();

                foreach (var order in ways)
                {
                    var key = string.Join(string.Empty, order.Select(x => x.Name).OrderBy(x => x));
                    var score = GetOrderScore(order, 26);
                    bestScoresDictionary[key] = (order, Math.Max(bestScoresDictionary.GetValueOrDefault(key, (null, 0)).Item2, score));
                }

                var bestScores = bestScoresDictionary.ToList();

                answer = 0;

                for (var h = 0; h < bestScores.Count; h++)
                {
                    for (var e = h + 1; e < bestScores.Count; e++)
                    {
                        var hs = bestScores[h];
                        var es = bestScores[e];

                        foreach (var valve in hs.Value.Item1)
                        {
                            if (es.Value.Item1.Contains(valve))
                            {
                                break;
                            }

                            answer = Math.Max(answer, hs.Value.Item2 + es.Value.Item2);
                        }
                    }
                }

                /*var allFlowRates = allValves.Select((x, i) => (x.FlowRate, x)).ToList();

                int RecursiveSolve(Valve current, int time, HashSet<Valve> visited, bool elephants)
                {
                    var copy = new HashSet<Valve>(visited) { current };

                    var best = 0;

                    foreach (var v in allUsefulValves)
                    {
                        var dist = optimizedFloydWarshall[current][v] + 1;

                        if (v.FlowRate == 0 || copy.Contains(v) || dist > time)
                        {
                            continue;
                        }

                        var score = v.FlowRate * (time - dist);

                        if (dist != time)
                        {
                            score += RecursiveSolve(v, time - dist, copy, elephants);
                        }

                        best = Math.Max(score, best);
                    }

                    if (elephants)
                    {
                        best = Math.Max(best, RecursiveSolve(start, 26, copy, false));
                    }

                    return best;
                }

                answer = RecursiveSolve(start, 26, new HashSet<Valve>(), true);*/

                /*var best = 0;
                const int maxTime = 26;

                void DFS(int score, Valve valve, HashSet<Valve> visited, int time, bool initial)
                {
                    best = Math.Max(best, score);

                    if (visited.Count >= optimizedFloydWarshall.Count)
                    {
                        return;
                    }

                    foreach (var (targetValve, distance) in optimizedFloydWarshall[valve])
                    {
                        if (visited.Contains(targetValve) || time + distance + 1 >= maxTime)
                        {
                            continue;
                        }

                        DFS(
                            score + (maxTime - time - distance - 1) * targetValve.FlowRate,
                            targetValve,
                            new HashSet<Valve>(visited) { targetValve },
                            time + distance + 1,
                            initial
                        );
                    }

                    if (initial)
                    {
                        DFS(score, start, visited, 0, false);
                    }
                }

                DFS(0, start, new HashSet<Valve>(), 0, true);*/
            }

            //Do a BFS
            /*var queue = new Queue<(int time, Valve location, Valve elephant, int score, HashSet<Valve> opened)>();
            queue.Enqueue((1, start, start, 0, new HashSet<Valve>()));

            var visited = new Dictionary<(int time, Valve location, Valve elephant), int>();
            var best = 0;

            var maxFlow = lines.Sum(x => x.valve.FlowRate);
            
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                
                if (visited.GetValueOrDefault((v.time, v.location, v.elephant), -1) >= v.score)
                {
                    continue;
                }
                
                visited[(v.time, v.location, v.elephant)] = v.score;

                if (v.time == 26)
                {
                    if (v.score > best)
                    {
                        best = v.score;
                    }
                    
                    continue;
                }
                
                var currentFlow = v.opened.Sum(x => x.FlowRate);

                //We are done before time!
                if (currentFlow >= maxFlow)
                {
                    var score = v.score + currentFlow;
                    var time = v.time;
                    
                    while (time < 25)
                    {
                        time += 1;
                        score += currentFlow;
                    }
                    
                    var opened = new HashSet<Valve>(v.opened);
                    var state = (time + 1, v.location, v.elephant, score, opened);
                    queue.Enqueue(state);
                    
                    continue;
                }
                
                //Open valve
                if (v.location.FlowRate > 0 && !v.opened.Contains(v.location))
                {
                    var opened = new HashSet<Valve>(v.opened)
                    {
                        v.location
                    };

                    //Elephant opens valve
                    if (v.elephant.FlowRate > 0 && !opened.Contains(v.elephant))
                    {
                        var openedWithElephant = new HashSet<Valve>(opened)
                        {
                            v.elephant
                        };
                        
                        var score = v.score + openedWithElephant.Sum(x => x.FlowRate);
                        var state = (v.time + 1, v.location, v.elephant, score, openedWithElephant);
                        queue.Enqueue(state);
                    }

                    //Elephant goes somewhere
                    {
                        var score = v.score + opened.Sum(x => x.FlowRate);

                        foreach (var tunnel in v.elephant.Tunnels)
                        {
                            var state = (
                                v.time + 1, v.location, tunnel, score, 
                                new HashSet<Valve>(opened)
                            );
                            queue.Enqueue(state);
                        }
                    }
                }
                
                //Go somewhere
                foreach (var tunnel in v.location.Tunnels)
                {
                    //Elephant opens valve
                    if (v.elephant.FlowRate > 0 && !v.opened.Contains(v.elephant))
                    {
                        var openedWithElephant = new HashSet<Valve>(v.opened)
                        {
                            v.elephant
                        };
                        
                        var score = v.score + openedWithElephant.Sum(x => x.FlowRate);
                        var state = (v.time + 1, tunnel, v.elephant, score, openedWithElephant);
                        queue.Enqueue(state);
                    }

                    //Elephant goes somewhere
                    {
                        var score = v.score + v.opened.Sum(x => x.FlowRate);

                        foreach (var tunnel2 in v.elephant.Tunnels)
                        {
                            var state = (
                                v.time + 1, tunnel, tunnel2, score, 
                                new HashSet<Valve>(v.opened)
                            );
                            queue.Enqueue(state);
                        }
                    }   
                }
            }*/

            //Too low: 2423
            //Too high: 2727
            //Answer: 2425
            Logger.Info($"Day 16B: {answer}");
        }

        private static Dictionary<(Valve a, Valve b), int> FloydWarshall(List<Valve> allValves)
        {
            const int INFINITE = 1 * (10 ^ 9);

            var floydWarshall = new Dictionary<(Valve a, Valve b), int>();

            foreach (var valve1 in allValves)
            {
                foreach (var valve2 in allValves)
                {
                    if (valve1 == valve2)
                    {
                        floydWarshall[(valve1, valve2)] = 0;
                    }
                    else if (valve1.Tunnels.Contains(valve2))
                    {
                        floydWarshall[(valve1, valve2)] = 1;
                    }
                    else
                    {
                        floydWarshall[(valve1, valve2)] = INFINITE;
                    }
                }
            }

            foreach (var k in allValves)
            {
                foreach (var i in allValves)
                {
                    foreach (var j in allValves)
                    {
                        floydWarshall[(i, j)] = Math.Min(floydWarshall[(i, j)], floydWarshall[(i, k)] + floydWarshall[(k, j)]);
                    }
                }
            }

            return floydWarshall;
        }

        private static Dictionary<Valve, Dictionary<Valve, int>> FloydWarshallAsDictionary(List<Valve> allValves, Dictionary<(Valve a, Valve b), int> floydWarshall)
        {
            var dictionary = new Dictionary<Valve, Dictionary<Valve, int>>();

            foreach (var valve1 in allValves)
            {
                if (valve1.Name != "AA" && valve1.FlowRate == 0)
                {
                    continue;
                }

                var valves = floydWarshall
                    .Where(x => x.Key.a == valve1)
                    .ToDictionary(x => x.Key.b, x => x.Value);

                dictionary.Add(valve1, valves);
            }

            return dictionary;
        }
    }
}
