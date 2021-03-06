﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdventOfCode2020.Days
{
    public static class Leaderboard
    {
        private class LeaderboardCompletionDay
        {
            [JsonPropertyName("get_star_ts")]
            public string TimeStamp { get; set; }
        }

        private class LeaderboardMember
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("completion_day_level")]
            public Dictionary<string, Dictionary<string, LeaderboardCompletionDay>> CompletionDayLevels { get; set; }

            [JsonPropertyName("local_score")]
            public int LocalScore { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("last_star_ts")]
            public string LastStarTimeStamp { get; set; }

            [JsonPropertyName("global_score")]
            public int GlobalScore { get; set; }

            [JsonPropertyName("stars")]
            public int Stars { get; set; }
        }

        private class LeaderboardData
        {
            [JsonPropertyName("owner_id")]
            public string Owner { get; set; }

            [JsonPropertyName("members")]
            public Dictionary<string, LeaderboardMember> Members { get; set; }

            [JsonPropertyName("event")]
            public string Event { get; set; }
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

        public static void Start()
        {
            var jsonString = File.ReadAllText("Content\\Leaderboard.json");
            var jsonData = JsonSerializer.Deserialize<LeaderboardData>(jsonString);

            var members = GetMemberInfo(jsonData);

            var result = PrintAllMemberInfo(members);
            Logger.Debug(result);

            result = ExportLocalScoreboardOriginal(members);
            File.WriteAllText("leaderboard-original.csv", result);

            result = ExportLocalScoreboardPart2(members);
            File.WriteAllText("leaderboard-part2.csv", result);
        }

        private static List<Member> GetMemberInfo(LeaderboardData leaderboardData)
        {
            List<Member> members = new List<Member>();

            foreach (var leaderboardMember in leaderboardData.Members.Values)
            {
                var name = leaderboardMember.Name ?? "#" + leaderboardMember.Id;

                var member = new Member
                {
                    Id = leaderboardMember.Id,
                    Name = name
                };

                members.Add(member);

                for (var i = 1; i <= 25; i++)
                {
                    var day = new Day();

                    member.Days.Add(day);

                    if (!leaderboardMember.CompletionDayLevels.TryGetValue(i.ToString(), out var completionDayKVP))
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
                    .Select((x, index) =>
                    {
                        var first = x.First();

                        var score1 = x.Sum(y => y.ScorePart1);
                        var score2 = x.Sum(y => y.ScorePart2);
                        var date1 = x.FirstOrDefault(y => y.DatePart1.HasValue)?.DatePart1;
                        var date2 = x.FirstOrDefault(y => y.DatePart2.HasValue)?.DatePart2;
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
                        => x.Value
                            .Where((y, i) => i > 0) //NOTE: Skip Day 1!
                            .Sum(y => y.Score)
                );

            StringBuilder csvFile = new StringBuilder();
            csvFile.Append(";;");

            for(int i = 0; i < 25; i++)
            {
                csvFile.Append($"Day {i + 1};;;;;");
            }

            csvFile.AppendLine();
            csvFile.Append("Member;Total Score;");

            for(int i = 0; i < 25; i++)
            {
                csvFile.Append("Part 1;;Part 2;;Difference;");
            }

            csvFile.AppendLine();

            foreach (var kvp in totalScoresPerMember.OrderByDescending(x => x.Value))
            {
                string memberId = kvp.Key;
                var member = members.First(x => x.Id == memberId);

                csvFile.Append($"{member.Name};");

                csvFile.Append($"{kvp.Value};");

                foreach (var scoreboardEntry in scoreboardEntriesPerMember[memberId])
                {
                    csvFile.Append($"{scoreboardEntry.DatePart1?.ToString("g") ?? string.Empty};{scoreboardEntry.ScorePart1};");
                    csvFile.Append($"{scoreboardEntry.DatePart2?.ToString("g") ?? string.Empty};{scoreboardEntry.ScorePart2};");
                    csvFile.Append($"{scoreboardEntry.Minutes?.ToString("F2") ?? string.Empty};");
                }

                csvFile.AppendLine();
            }

            return csvFile.ToString();
        }

        private static string ExportLocalScoreboardPart2(List<Member> members)
        {
            return string.Empty;
        }
    }
}
