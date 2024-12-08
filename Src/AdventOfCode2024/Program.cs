using AdventOfCode2024.Days;

namespace AdventOfCode2024
{
    public static class Program
    {
        public static void Main()
        {
            if (Constants.UpdateLeaderboard)
            {
                Leaderboard.Start();
            }

            Logger.ShowDebug = false;

            if (Constants.RunCompletedDays)
            {
                RunDays(Constants.CompletedDays);
            }

            Logger.ShowDebug = true;

            RunDay(Constants.ActiveDay);
        }

        private static void RunDays(List<IDay> days)
        {
            foreach (var day in days)
            {
                RunDay(day);
            }
        }

        private static void RunDay(IDay day)
        {
            if (day == null)
            {
                return;
            }

            if (day is IDayA dayA)
            {
                dayA.StartA();
            }

            if (day is IDayB dayB)
            {
                dayB.StartB();
            }
        }
    }
}