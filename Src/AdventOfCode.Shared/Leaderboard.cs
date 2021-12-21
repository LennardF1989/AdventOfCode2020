using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AdventOfCode.Shared
{
    public static class Leaderboard
    {
        private class LeaderboardCompletionDay
        {
            [JsonProperty("get_star_ts")]
            public string TimeStamp { get; set; }
        }

        private class LeaderboardMember
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("completion_day_level")]
            public Dictionary<string, Dictionary<string, LeaderboardCompletionDay>> CompletionDayLevels { get; set; }

            [JsonProperty("local_score")]
            public int LocalScore { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("last_star_ts")]
            public string LastStarTimeStamp { get; set; }

            [JsonProperty("global_score")]
            public int GlobalScore { get; set; }

            [JsonProperty("stars")]
            public int Stars { get; set; }
        }

        private class LeaderboardData
        {
            [JsonProperty("owner_id")]
            public string Owner { get; set; }

            [JsonProperty("members")]
            public Dictionary<string, LeaderboardMember> Members { get; set; }

            [JsonProperty("event")]
            public string Event { get; set; }
        }

        private class LeaderboardSettings
        {
            [JsonProperty("idMapping")]
            public Dictionary<string, string> IdMapping { get; set; }

            [JsonProperty("mergeMapping")]
            public Dictionary<string, string> MergeMapping { get; set; }

            public LeaderboardSettings()
            {
                IdMapping = new Dictionary<string, string>();
                MergeMapping = new Dictionary<string, string>();
            }
        }

        private class Day
        {
            public DateTime? Part1 { get; set; }
            public DateTime? Part2 { get; set; }
        }

        private class Member
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public List<Day> Days { get; set; }

            public bool IsAnonymous { get; set; }

            public Member()
            {
                Days = new List<Day>();
            }
        }

        private class ScoreboardEntry
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int DayIndex { get; set; }
            public int Stars { get; set; }
            public DateTime? DatePart1 { get; set; }
            public DateTime? DatePart2 { get; set; }
            public double? Minutes { get; set; }
            public int ScorePart1 { get; set; }
            public int ScorePart2 { get; set; }
            public int Score { get; set; }
        }

        private const string LEADERBOARD_JSON = "Content\\Leaderboard.json";
        private const string LEADERBOARD_SETTINGS_JSON = "Content\\Leaderboard_Settings.json";

        public static void Start()
        {
            if (!File.Exists(LEADERBOARD_JSON))
            {
                return;
            }

            var leaderboardData = JsonConvert.DeserializeObject<LeaderboardData>(
                File.ReadAllText(LEADERBOARD_JSON)
            );

            LeaderboardSettings leaderboardSettings;

            if (File.Exists(LEADERBOARD_SETTINGS_JSON))
            {
                leaderboardSettings = JsonConvert.DeserializeObject<LeaderboardSettings>(
                    File.ReadAllText(LEADERBOARD_SETTINGS_JSON)
                );
            }
            else
            {
                leaderboardSettings = new LeaderboardSettings();
            }

            var members = GetMemberInfo(leaderboardData, leaderboardSettings);

            var result = PrintAllMemberInfo(members);

            Logger.Debug(result);

            members = members
                //.Where(x => !x.IsAnonymous)
                .ToList();

            result = ExportLocalScoreboardOriginal(members);

            File.WriteAllText("leaderboard-original.csv", result);
        }

        private static List<Member> GetMemberInfo(
            LeaderboardData leaderboardData,
            LeaderboardSettings leaderboardSettings
        )
        {
            List<Member> members = new List<Member>();

            foreach (var leaderboardMember in leaderboardData.Members.Values)
            {
                if (!leaderboardSettings.IdMapping.TryGetValue(leaderboardMember.Id, out var name))
                {
                    name = leaderboardMember.Name ?? "#" + leaderboardMember.Id;
                }

                var member = new Member
                {
                    Id = leaderboardMember.Id,
                    Name = name,

                    IsAnonymous = leaderboardMember.Name == null
                };

                members.Add(member);

                for (var i = 1; i <= 25; i++)
                {
                    var day = new Day();

                    member.Days.Add(day);

                    if (leaderboardMember.CompletionDayLevels == null || !leaderboardMember.CompletionDayLevels.TryGetValue(i.ToString(), out var completionDayKVP))
                    {
                        continue;
                    }

                    if (completionDayKVP.TryGetValue("1", out var day1))
                    {
                        var date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(day1.TimeStamp));
                        day.Part1 = date.DateTime;
                    }

                    if (completionDayKVP.TryGetValue("2", out var day2))
                    {
                        var date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(day2.TimeStamp));
                        day.Part2 = date.DateTime;
                    }
                }
            }

            foreach (var merge in leaderboardSettings.MergeMapping)
            {
                var member1 = members.FirstOrDefault(x => x.Id == merge.Key);
                var member2 = members.FirstOrDefault(x => x.Id == merge.Value);

                if (member1 == null || member2 == null)
                {
                    continue;
                }

                var newMember = new Member
                {
                    Name = member1.Name,
                    Id = member1.Id
                };

                for (var i = 0; i < 25; i++)
                {
                    var day1 = member1.Days[i];
                    var day2 = member2.Days[i];

                    double minutes1 = double.MaxValue;
                    double minutes2 = double.MaxValue;

                    if (day1.Part1.HasValue && day1.Part2.HasValue)
                    {
                        minutes1 = (day1.Part2.Value - day1.Part1.Value).TotalMinutes;
                    }

                    if (day2.Part1.HasValue && day2.Part2.HasValue)
                    {
                        minutes2 = (day2.Part2.Value - day2.Part1.Value).TotalMinutes;
                    }

                    if (minutes1 > minutes2)
                    {
                        newMember.Days.Add(member1.Days[i]);
                    }
                    else
                    {
                        newMember.Days.Add(member2.Days[i]);
                    }
                }

                members.Remove(member1);
                members.Remove(member2);
                members.Add(newMember);
            }

            return members;
        }

        private static string PrintAllMemberInfo(List<Member> members)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var member in members)
            {
                stringBuilder.AppendLine(member.Name);
                stringBuilder.AppendLine(string.Join(string.Empty, Enumerable.Repeat("-", member.Name.Length)));

                for (var i = 0; i < member.Days.Count; i++)
                {
                    var day = member.Days[i];

                    string dayString = $"Day {i + 1}";

                    if (!day.Part1.HasValue)
                    {
                        stringBuilder.AppendLine($"{dayString}: Not completed!");

                        continue;
                    }

                    stringBuilder.AppendLine($"{dayString}");
                    stringBuilder.AppendLine($"- Part 1 @ {day.Part1}");

                    if (day.Part2.HasValue)
                    {
                        stringBuilder.AppendLine($"- Part 2 @ {day.Part2}");

                        double minutes = (day.Part2.Value - day.Part1.Value).TotalMinutes;

                        stringBuilder.AppendLine($"- Difference: {minutes} minute(s)");
                    }
                    else
                    {
                        stringBuilder.AppendLine("- Part 2: Not completed!");
                    }
                }

                stringBuilder.AppendLine(string.Empty);
            }

            return stringBuilder.ToString();
        }

        private static string ExportLocalScoreboardOriginal(List<Member> members)
        {
            var scoreboardEntriesPerDay = new List<List<ScoreboardEntry>>();

            for (int i = 0; i < members[0].Days.Count; i++)
            {
                var scoresDay1 = members
                    .Where(x => x.Days[i].Part1.HasValue)
                    .OrderBy(x => x.Days[i].Part1)
                    .Select((x, index) => new ScoreboardEntry
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DayIndex = i,
                        Stars = 1,
                        DatePart1 = x.Days[i].Part1,
                        ScorePart1 = members.Count - index
                    })
                    .ToList();

                var scoresDay2 = members
                    .Where(x => x.Days[i].Part2.HasValue)
                    .OrderBy(x => x.Days[i].Part2)
                    .Select((x, index) => new ScoreboardEntry
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DayIndex = i,
                        Stars = 1,
                        DatePart2 = x.Days[i].Part2,
                        ScorePart2 = members.Count - index
                    })
                    .ToList();

                var scoresTotal = scoresDay1
                    .Union(scoresDay2)
                    .GroupBy(x => x.Id)
                    .Select(x =>
                    {
                        var first = x.First();

                        var score1 = x.Sum(y => y.ScorePart1);
                        var score2 = x.Sum(y => y.ScorePart2);
                        var date1 = x.FirstOrDefault(y => y.DatePart1.HasValue)?.DatePart1?.ToLocalTime();
                        var date2 = x.FirstOrDefault(y => y.DatePart2.HasValue)?.DatePart2?.ToLocalTime();
                        var minutes = date1.HasValue && date2.HasValue ? (double?)(date2 - date1).Value.TotalMinutes : null;

                        return new ScoreboardEntry
                        {
                            Id = first.Id,
                            Name = first.Name,
                            DayIndex = first.DayIndex,
                            Stars = x.Sum(y => y.Stars),
                            DatePart1 = date1,
                            ScorePart1 = score1,
                            DatePart2 = date2,
                            ScorePart2 = score2,
                            Minutes = minutes,
                            Score = score1 + score2
                        };
                    })
                    .ToList();

                scoreboardEntriesPerDay.Add(scoresTotal);
            }

            var scoreboardEntriesPerMember = scoreboardEntriesPerDay
                .SelectMany(x => x)
                .GroupBy(x => x.Id)
                .Select(x =>
                    x.OrderBy(y => y.DayIndex).ToList()
                )
                .ToDictionary(x => x.First().Id, x => x);

            var totalScoresPerMember = scoreboardEntriesPerMember
                .ToDictionary(
                    x => x.Key, x
                        => x.Value.Sum(y => y.Score)
                );

            StringBuilder csvFile = new StringBuilder();
            csvFile.Append(";;");

            for (int i = 0; i < 25; i++)
            {
                csvFile.Append($"Day {i + 1};;;;;;");
            }

            csvFile.AppendLine();
            csvFile.Append("Member;Total Score;");

            for (int i = 0; i < 25; i++)
            {
                csvFile.Append("Part 1;;Part 2;;Difference;Total;");
            }

            csvFile.AppendLine();

            foreach (var kvp in totalScoresPerMember.OrderByDescending(x => x.Value))
            {
                string memberId = kvp.Key;
                var member = members.First(x => x.Id == memberId);

                csvFile.Append($"{member.Name};");

                csvFile.Append($"{kvp.Value};");

                int dayCount = 0;
                foreach (var scoreboardEntry in scoreboardEntriesPerMember[memberId].OrderBy(x => x.DayIndex))
                {
                    while (scoreboardEntry.DayIndex > dayCount)
                    {
                        csvFile.Append(";;;;;;");

                        dayCount++;
                    }

                    csvFile.Append($"{scoreboardEntry.DatePart1?.ToString("g") ?? string.Empty};{scoreboardEntry.ScorePart1};");
                    csvFile.Append($"{scoreboardEntry.DatePart2?.ToString("g") ?? string.Empty};{scoreboardEntry.ScorePart2};");
                    csvFile.Append($"{scoreboardEntry.Minutes?.ToString("F2") ?? string.Empty};");
                    csvFile.Append($"{scoreboardEntry.ScorePart1 + scoreboardEntry.ScorePart2};");

                    dayCount++;
                }

                csvFile.AppendLine();
            }

            return csvFile.ToString();
        }
    }
}
