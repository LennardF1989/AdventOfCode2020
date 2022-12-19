using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day19
    {
        private record Blueprint(
            int number,
            int oreRobotOreCost,
            int clayRobotOreCost,
            int obsidianRobotOreCost,
            int obsidianRobotClayCost,
            int geodeRobotOreCost,
            int geodeRobotObsidianCost
        )
        {
            private readonly int _maxOre = Math.Max(oreRobotOreCost,
                Math.Max(clayRobotOreCost,
                    Math.Max(
                        geodeRobotOreCost,
                        obsidianRobotOreCost
                    )
                )
            );

            public int MaxOre()
            {
                return _maxOre;
            }
        }

        private record State(
            int ore,
            int clay,
            int obsidian,
            int geode,
            int oreRobot,
            int clayRobot,
            int obsidianRobot,
            int geodeRobot
        )
        {
            //NOTE: https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF
            public int CalculatePotentialScore(int timeLeft)
            {
                return geode + (geodeRobot * timeLeft) + (timeLeft * (timeLeft - 1) / 2);
            }

            public int CalculatePotentialScore2(int timeLeft)
            {
                return obsidian + (obsidianRobot * timeLeft) + (timeLeft * (timeLeft - 1) / 2);
            }
        }

        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day19_Test.txt")
                .ReadAllLines("Content\\Day19.txt")
                .Select(ParseInput)
                .ToList();

            var initialState = new State(0, 0, 0, 0, 1, 0, 0, 0);
            var score = 0;

            foreach (var blueprint in lines)
            {
                Logger.Debug($"Blueprint #{blueprint.number}");

                var result = SimulateBlueprint(blueprint, initialState, 24);
                
                Logger.Debug(result);

                score += (result * blueprint.number);
            }

            var answer = score;
            
            Logger.Info($"Day 19A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day19_Test.txt")
                .ReadAllLines("Content\\Day19.txt")
                .Select(ParseInput)
                .ToList();

            var initialState = new State(0, 0, 0, 0, 1, 0, 0, 0);
            var score = 1;

            foreach (var blueprint in lines.Take(3))
            {
                Logger.Debug($"Blueprint #{blueprint.number}");

                var result = SimulateBlueprint(blueprint, initialState, 32);

                Logger.Debug(result);

                score *= result;
            }

            var answer = score;

            Logger.Info($"Day 19B: {answer}");
        }

        private static Blueprint ParseInput(string x)
        {
            var numbers = Regex.Matches(x, "\\d+");

            return new Blueprint(int.Parse(numbers[0].Value), int.Parse(numbers[1].Value), int.Parse(numbers[2].Value), int.Parse(numbers[3].Value), int.Parse(numbers[4].Value), int.Parse(numbers[5].Value), int.Parse(numbers[6].Value));
        }

        //Disclaimer: Borrowed some optimizations from https://github.com/agubelu/Advent-of-Code-2022/blob/master/src/days/day19.rs
        private static int SimulateBlueprint(
            Blueprint blueprint, State state, int minute, 
            bool couldHaveBuildOreRobot = false, bool couldHaveBuildClayRobot = false, bool couldHaveBuildObsidianRobot = false, 
            int bestScoreSoFar = 0
        )
        {
            var (oreCount, clayCount, obsidianCount, geodeCount, _, _, _, _) = state;

            //Optimization: With only 1 minute to go, there is no use in building another robot, so finish it now.
            if (minute == 1)
            {
                return geodeCount + state.geodeRobot;
            }

            //Optimization: If the current geode score is less than the best we've seen so far, exit early.
            if (state.CalculatePotentialScore(minute) < bestScoreSoFar)
            {
                return 0;
            }

            //Optimization: If we can no longer build more obsidian robots in the time left, finish it now.
            if (state.CalculatePotentialScore2(minute) < blueprint.geodeRobotObsidianCost)
            {
                return geodeCount + state.geodeRobot * minute;
            }

            if (oreCount >= blueprint.geodeRobotOreCost && obsidianCount >= blueprint.geodeRobotObsidianCost)
            {
                var stateCopy = state with
                {
                    ore = (state.ore - blueprint.geodeRobotOreCost) + state.oreRobot,
                    clay = state.clay + state.clayRobot,
                    obsidian = (state.obsidian - blueprint.geodeRobotObsidianCost) + state.obsidianRobot,
                    geode = state.geode + state.geodeRobot,
                    geodeRobot = state.geodeRobot + 1
                };

                return SimulateBlueprint(
                    blueprint, stateCopy, minute - 1,
                    bestScoreSoFar: bestScoreSoFar
                );
            }

            var bestScore = bestScoreSoFar;

            //Optimization: If we could have build a robot, but didn't, we want to keep that streak going.
            var didBuildOreRobot = false;
            var didBuildClayRobot = false;
            var didBuildObsidianRobot = false;

            var enoughObsidianPerMinute = state.obsidianRobot == blueprint.geodeRobotObsidianCost;

            if (!couldHaveBuildObsidianRobot && !enoughObsidianPerMinute && oreCount >= blueprint.obsidianRobotOreCost && clayCount >= blueprint.obsidianRobotClayCost)
            {
                var stateCopy = state with
                {
                    ore = (state.ore - blueprint.obsidianRobotOreCost) + state.oreRobot,
                    clay = (state.clay - blueprint.obsidianRobotClayCost) + state.clayRobot,
                    obsidian = state.obsidian + state.obsidianRobot,
                    geode = state.geode + state.geodeRobot,
                    obsidianRobot = state.obsidianRobot + 1
                };

                var result = SimulateBlueprint(
                    blueprint, stateCopy, minute - 1,
                    bestScoreSoFar: bestScore
                );

                bestScore = Math.Max(result, bestScore);

                didBuildObsidianRobot = true;
            }

            var enoughClayPerMinute = state.clayRobot == blueprint.obsidianRobotClayCost;

            if (!couldHaveBuildClayRobot && !enoughClayPerMinute && oreCount >= blueprint.clayRobotOreCost)
            {
                var stateCopy = state with
                {
                    ore = (state.ore - blueprint.clayRobotOreCost) + state.oreRobot,
                    clay = state.clay + state.clayRobot,
                    obsidian = state.obsidian + state.obsidianRobot,
                    geode = state.geode + state.geodeRobot,
                    clayRobot = state.clayRobot + 1
                };

                var result = SimulateBlueprint(
                    blueprint, stateCopy, minute - 1, 
                    bestScoreSoFar: bestScore
                );

                bestScore = Math.Max(result, bestScore);

                didBuildClayRobot = true;
            }

            var enoughOrePerMinute = state.oreRobot == blueprint.MaxOre();

            if (!couldHaveBuildOreRobot && !enoughOrePerMinute && oreCount >= blueprint.oreRobotOreCost)
            {
                var stateCopy = state with
                {
                    ore = (state.ore - blueprint.oreRobotOreCost) + state.oreRobot,
                    clay = state.clay + state.clayRobot,
                    obsidian = state.obsidian + state.obsidianRobot,
                    geode = state.geode + state.geodeRobot,
                    oreRobot = state.oreRobot + 1
                };

                var result = SimulateBlueprint(
                    blueprint, stateCopy, minute - 1,
                    bestScoreSoFar: bestScore
                );

                bestScore = Math.Max(result, bestScore);

                didBuildOreRobot = true;
            }

            //Don't buy anything
            {
                var stateCopy = state with
                {
                    ore = state.ore + state.oreRobot,
                    clay = state.clay + state.clayRobot,
                    obsidian = state.obsidian + state.obsidianRobot,
                    geode = state.geode + state.geodeRobot
                };

                var result = SimulateBlueprint(
                    blueprint, stateCopy, minute - 1, 
                    didBuildOreRobot, didBuildClayRobot, didBuildObsidianRobot,
                    bestScore
                );

                bestScore = Math.Max(result, bestScore);
            }

            return bestScore;
        }
    }
}
