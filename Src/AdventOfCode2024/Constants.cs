using AdventOfCode2024.Days;

namespace AdventOfCode2024
{
    public static class Constants
    {
        public const bool UpdateLeaderboard = false;
        public const bool RunCompletedDays = false;

        public static readonly List<IDay> CompletedDays =
        [
            new Day01(),
            new Day02(),
            new Day03(),
            new Day04(),
            new Day05(),
            new Day06(),
            new Day07(),
            new Day08()
        ];

        public static readonly IDay ActiveDay = new Day09();
    }
}