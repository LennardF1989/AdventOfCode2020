using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day14
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day14_Test.txt");
            var lines = File.ReadAllLines("Content\\Day14.txt");

            var mask = string.Empty;
            var memory = new Dictionary<long, long>();

            foreach (string line in lines)
            {
                if(line.StartsWith("mask = "))
                {
                    mask = Regex.Match(line, "mask = ([01X]+)").Groups[1].Value;
                }
                else
                {
                    var match = Regex.Match(line, "mem\\[(\\d+)\\] = (\\d+)");
                    var memoryAddress = int.Parse(match.Groups[1].Value);
                    var value = long.Parse(match.Groups[2].Value);

                    Logger.Debug(Convert.ToString(value, 2).PadLeft(36, '0'));

                    for (var i = 0; i < mask.Length; i++)
                    {
                        char c = mask[mask.Length - 1 - i];

                        if (c == '0')
                        {
                            value &= ~(1L << i);
                        }
                        else if (c == '1')
                        {
                            value |= (1L << i);
                        }
                        else if (c == 'X')
                        {
                            //Do nothing
                        }
                    }

                    if (!memory.ContainsKey(memoryAddress))
                    {
                        memory.Add(memoryAddress, 0);
                    }

                    memory[memoryAddress] = value;

                    Logger.Debug(Convert.ToString(value, 2).PadLeft(36, '0'));
                }
            }

            long answer = memory.Values.Sum();

            Logger.Info($"Day 14A: {answer}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day14_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day14.txt");

            var mask = string.Empty;
            var memory = new Dictionary<long, long>();
            
            long numberOfCombinations = 0;

            foreach (string line in lines)
            {
                if(line.StartsWith("mask = "))
                {
                    mask = Regex.Match(line, "mask = ([01X]+)").Groups[1].Value;
                    numberOfCombinations = (long) BigInteger.Pow(2, mask.Count(x => x == 'X'));
                }
                else
                {
                    var match = Regex.Match(line, "mem\\[(\\d+)\\] = (\\d+)");
                    var memoryAddress = long.Parse(match.Groups[1].Value);
                    var value = long.Parse(match.Groups[2].Value);

                    for (long i = 0; i < numberOfCombinations; i++)
                    {
                        long memoryAddressCopy = memoryAddress;

                        int offset = 0;
                        for (var x = 0; x < mask.Length; x++)
                        {
                            char c = mask[mask.Length - 1 - x];

                            if (c == '0')
                            {
                                //Do nothing
                            }
                            else if (c == '1')
                            {
                                memoryAddressCopy |= (1L << x);
                            }
                            else if (c == 'X')
                            {
                                var onOrOff = (i >> offset) & 1;

                                if (onOrOff == 0)
                                {
                                    memoryAddressCopy &= ~(1L << x);
                                }
                                else
                                {
                                    memoryAddressCopy |= (1L << x);
                                }

                                offset++;
                            }
                        }

                        if (!memory.ContainsKey(memoryAddressCopy))
                        {
                            memory.Add(memoryAddressCopy, 0);
                        }

                        memory[memoryAddressCopy] = value;
                    }
                }
            }

            long answer = memory.Values.Sum();

            Logger.Info($"Day 14B: {answer}");
        }
    }
}
