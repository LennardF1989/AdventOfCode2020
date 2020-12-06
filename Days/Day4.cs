using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public static class Day4
    {
        private const string BirthYear = "byr";
        private const string IssueYear = "iyr";
        private const string ExpirationYear = "eyr";
        private const string Height = "hgt";
        private const string HairColor = "hcl";
        private const string EyeColor = "ecl";
        private const string PassportId = "pid";
        //private const string CountryId = "cid";

        private static readonly string[] Fields = 
        {
            BirthYear,
            IssueYear,
            ExpirationYear, 
            Height, 
            HairColor, 
            EyeColor, 
            PassportId
            //CountryId
        };

        private static readonly Dictionary<string, Func<string, bool>> FieldValidations = new Dictionary<string, Func<string, bool>>
        {
            {BirthYear, ValidateBirthYear},
            {IssueYear, ValidateIssueYear},
            {ExpirationYear, ValidateExpirationYear},
            {Height, ValidateHeight},
            {HairColor, ValidateHairColor},
            {EyeColor, ValidateEyeColor},
            {PassportId, ValidatePassportId}
            //{CountryId, s => true}
        };

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day4_Test.txt");
            var lines = File.ReadAllLines("Content\\Day4.txt");

            StringBuilder stringBuilder = new StringBuilder();

            int valid = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (!string.IsNullOrEmpty(line))
                {
                    stringBuilder.Append(line + " ");

                    //NOTE: Don't continue on EOF
                    if (i < lines.Length - 1)
                    {
                        continue;
                    }
                }

                var passportLine = stringBuilder.ToString();
                stringBuilder.Clear();

                Logger.Debug(passportLine);

                bool isValid = true;
                foreach (var field in Fields)
                {
                    if (!passportLine.Contains(field + ":"))
                    {
                        isValid = false;

                        break;
                    }
                }

                if (isValid)
                {
                    valid++;
                }
            }

            Logger.Info($"Day 4A: {valid}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day4_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day4.txt");

            StringBuilder stringBuilder = new StringBuilder();

            int valid = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (!string.IsNullOrEmpty(line))
                {
                    stringBuilder.Append(line + " ");

                    //NOTE: Don't continue on EOF
                    if (i < lines.Length - 1)
                    {
                        continue;
                    }
                }

                var passportLine = stringBuilder.ToString();
                stringBuilder.Clear();

                var allFields = passportLine
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(":"))
                    .ToDictionary(x => x[0], x => x[1]);

                bool isValid = true;

                foreach (var field in FieldValidations.Keys)
                {
                    if (allFields.ContainsKey(field) && 
                        FieldValidations[field](allFields[field]))
                    {
                        continue;
                    }

                    isValid = false;

                    break;
                }

                if (isValid)
                {
                    valid++;
                }
            }

            Logger.Info($"Day 4B: {valid}");
        }

        private static bool ValidateBirthYear(string input)
        {
            if (int.TryParse(input, out var year))
            {
                return year >= 1920 && year <= 2002;
            }

            return false;
        }

        private static bool ValidateIssueYear(string input)
        {
            if (int.TryParse(input, out var year))
            {
                return year >= 2010 && year <= 2020;
            }

            return false;
        }

        private static bool ValidateExpirationYear(string input)
        {
            if (int.TryParse(input, out var year))
            {
                return year >= 2020 && year <= 2030;
            }

            return false;
        }

        private static bool ValidateHeight(string input)
        {
            int height;

            if (input.EndsWith("cm") && int.TryParse(input.Substring(0, input.Length - 2), out height))
            {
                return height >= 150 && height <= 193;
            }

            if (input.EndsWith("in") && int.TryParse(input.Substring(0, input.Length - 2), out height))
            {
                return height >= 59 && height <= 76;
            }

            return false;
        }

        private static bool ValidateHairColor(string input)
        {
            return Regex.IsMatch(input, "^#[0-9a-f]{6}$");
        }

        private static bool ValidateEyeColor(string input)
        {
            return Regex.IsMatch(input, "^amb|blu|brn|gry|grn|hzl|oth$");
        }

        private static bool ValidatePassportId(string input)
        {
            return Regex.IsMatch(input, "^[0-9]{9}$");
        }
    }
}
