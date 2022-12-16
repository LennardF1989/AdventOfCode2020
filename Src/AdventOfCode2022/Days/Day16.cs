using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

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
                    .ReadAllLines("Content//Day16_Test.txt")
                    //.ReadAllLines("Content//Day16.txt")
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
            var queue = new Queue<(int time, Valve location, Valve elephant, int score, HashSet<Valve> opened)>();
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
                    best = Math.Max(best, v.score);
                    
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
                    var state = (v.time + 1, v.location, v.elephant, score, opened);
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
                                v.time + 1, tunnel, v.elephant, score, 
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
                            var opened = new HashSet<Valve>(v.opened);
                            var state = (v.time + 1, tunnel, tunnel2, score, opened);
                            queue.Enqueue(state);
                        }
                    }   
                }
            }

            var answer = best;
            
            Logger.Info($"Day 16B: {answer}");
        }
    }
}
