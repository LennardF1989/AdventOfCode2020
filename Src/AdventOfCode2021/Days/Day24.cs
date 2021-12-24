using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day24
    {
        enum Instruction
        {
            None = 0,
            Input,
            Add,
            Multiply,
            Divide,
            Modulo,
            Equals
        }

        //Disclaimer: Decompiled the full input myself, but found that non-bruteforce online.
        public static void Start()
        {
            var lines = File
                .ReadAllLines("Content\\Day24.txt")
                .Select(ParseInput)
                .ToList();

            //NOTE: Found 91131616112785 by bruteforce
            char[] modelNumber = "91131616112785".ToCharArray();
            var result = RunALU(lines, modelNumber);
            result = FastALU(modelNumber);
            result = EvenFasterALU(modelNumber);

            var answer = SolveALU();

            Logger.Info($"Day 24A: {answer.Item1}");
            Logger.Info($"Day 24B: {answer.Item2}");
        }

        //Source: Based on https://gist.github.com/thatsumoguy/7d7c58b21dde594cf127cbf2b167f2f9
        private static (long, long) SolveALU()
        {
            var checks = new[]
            {
                11, 14, 10, 0, 12, 12, 12, -8, -9, 11, 0, -5, -6, -12
            };
            var offsets = new[]
            {
                8, 13, 2, 7, 11, 4, 13, 13, 10, 1, 2, 14, 6, 14
            };

            var stack = new Stack<(int, int)>();
            var keys = new Dictionary<int, (int x, int y)>();

            for (int i = 0; i < 14; i++)
            {
                var pair = (checks[i], offsets[i]);

                if (pair.Item1 > 0)
                {
                    stack.Push((i, pair.Item2));
                }
                else
                {
                    var (check, offset) = stack.Pop();
                    keys[i] = (check, offset + pair.Item1);
                }
            }

            var maxOutput = new Dictionary<int, int>();
            var minOutput = new Dictionary<int, int>();

            foreach (var (key, val) in keys)
            {
                maxOutput[key] = Math.Min(9, 9 + val.y);
                maxOutput[val.x] = Math.Min(9, 9 - val.y);

                minOutput[key] = Math.Max(1, 1 + val.y);
                minOutput[val.x] = Math.Max(1, 1 - val.y);
            }

            var min = long.Parse(
                string.Join(
                    string.Empty,
                    maxOutput
                        .OrderBy(x => x.Key)
                        .Select(x => x.Value)
                )
            );

            var max = long.Parse(
                string.Join(
                    string.Empty,
                    minOutput
                        .OrderBy(x => x.Key)
                        .Select(x => x.Value)
                )
            );

            return (min, max);
        }

        //Source: Based on description in https://github.com/dphilipson/advent-of-code-2021/blob/master/src/days/day24.rs
        private static int EvenFasterALU(char[] inputs)
        {
            /*var divisions = new[]
            {
                1, 1, 1, 26, 1, 1, 1, 26, 26, 1, 26, 26, 26, 26
            };*/
            var checks = new[]
            {
                11, 14, 10, 0, 12, 12, 12, -8, -9, 11, 0, -5, -6, -12
            };
            var offsets = new[]
            {
                8, 13, 2, 7, 11, 4, 13, 13, 10, 1, 2, 14, 6, 14
            };

            var stack = new Stack<int>();

            for (var i = 0; i < inputs.Length; i++)
            {
                int input = int.Parse(inputs[i].ToString());

                if (checks[i] > 0)
                {
                    stack.Push(input + offsets[i]);
                }
                else
                {
                    var value = stack.Pop();

                    if (value + checks[i] != input)
                    {
                        return -1;
                    }
                }
            }

            return stack.Count;
        }

        private static int FastALU(char[] input)
        {
            var v1 = int.Parse(input[0].ToString());
            var v2 = int.Parse(input[1].ToString());
            var v3 = int.Parse(input[2].ToString());
            var v4 = int.Parse(input[3].ToString());
            var v5 = int.Parse(input[4].ToString());
            var v6 = int.Parse(input[5].ToString());
            var v7 = int.Parse(input[6].ToString());
            var v8 = int.Parse(input[7].ToString());
            var v9 = int.Parse(input[8].ToString());
            var v10 = int.Parse(input[9].ToString());
            var v11 = int.Parse(input[10].ToString());
            var v12 = int.Parse(input[11].ToString());
            var v13 = int.Parse(input[12].ToString());
            var v14 = int.Parse(input[13].ToString());

            var p0 = 0;

            var check1 = ((p0 % 26) + 11 == v1 ? 1 : 0) == 0 ? 1 : 0;
            var p1 = (p0 / 1) * ((25 * check1) + 1) + ((v1 + 8) * check1);

            var check2 = ((p1 % 26) + 14 == v2 ? 1 : 0) == 0 ? 1 : 0;
            var p2 = ((p1 / 1) * ((25 * check2) + 1)) + ((v2 + 13) * check2);

            var check3 = ((p2 % 26) + 10 == v3 ? 1 : 0) == 0 ? 1 : 0;
            var p3 = ((p2 / 1) * ((25 * check3) + 1)) + ((v3 + 2) * check3);

            var check4 = ((p3 % 26) + 0 == v4 ? 1 : 0) == 0 ? 1 : 0;
            var p4 = ((p3 / 26) * ((25 * check4) + 1)) + ((v4 + 7) * check4);

            var check5 = ((p4 % 26) + 12 == v5 ? 1 : 0) == 0 ? 1 : 0;
            var p5 = ((p4 / 1) * ((25 * check5) + 1)) + ((v5 + 11) * check5);

            var check6 = ((p5 % 26) + 12 == v6 ? 1 : 0) == 0 ? 1 : 0;
            var p6 = ((p5 / 1) * ((25 * check6) + 1)) + ((v6 + 4) * check6);

            var check7 = ((p6 % 26) + 12 == v7 ? 1 : 0) == 0 ? 1 : 0;
            var p7 = ((p6 / 1) * ((25 * check7) + 1)) + ((v7 + 13) * check7);

            var check8 = ((p7 % 26) + (-8) == v8 ? 1 : 0) == 0 ? 1 : 0;
            var p8 = ((p7 / 26) * ((25 * check8) + 1)) + ((v8 + 13) * check8);

            var check9 = ((p8 % 26) + (-9) == v9 ? 1 : 0) == 0 ? 1 : 0;
            var p9 = ((p8 / 26) * ((25 * check9) + 1)) + ((v9 + 10) * check9);

            var check10 = ((p9 % 26) + 11 == v10 ? 1 : 0) == 0 ? 1 : 0;
            var p10 = ((p9 / 1) * ((25 * check10) + 1)) + ((v10 + 1) * check10);

            var check11 = ((p10 % 26) + 0 == v11 ? 1 : 0) == 0 ? 1 : 0;
            var p11 = ((p10 / 26) * ((25 * check11) + 1)) + ((v11 + 2) * check11);

            var check12 = ((p11 % 26) + (-5) == v12 ? 1 : 0) == 0 ? 1 : 0;
            var p12 = ((p11 / 26) * ((25 * check12) + 1)) + ((v12 + 14) * check12);

            var check13 = ((p12 % 26) + (-6) == v13 ? 1 : 0) == 0 ? 1 : 0;
            var p13 = ((p12 / 26) * ((25 * check13) + 1)) + ((v13 + 6) * check13);

            var check14 = ((p13 % 26) + (-12) == v14 ? 1 : 0) == 0 ? 1 : 0;
            var p14 = ((p13 / 26) * ((25 * check14) + 1)) + ((v14 + 14) * check14);

            return p14;
        }

        private static int RunALU(List<(Instruction instruction, string[] arguments)> lines, char[] input)
        {
            var inputs = new Stack<int>(
                input.Select(x => int.Parse(x.ToString())).Reverse().ToArray()
            );

            Dictionary<string, int> variables = new Dictionary<string, int>
            {
                { "w", 0 },
                { "x", 0 },
                { "y", 0 },
                { "z", 0 }
            };

            foreach (var opcode in lines)
            {
                if (opcode.instruction == Instruction.None)
                {
                    continue;
                }

                if (opcode.instruction == Instruction.Input)
                {
                    var value = inputs.Pop();

                    variables[opcode.arguments[0]] = value;

                    continue;
                }

                var a = variables[opcode.arguments[0]];
                var b = IsVariable(opcode.arguments[1])
                    ? variables[opcode.arguments[1]]
                    : int.Parse(opcode.arguments[1]);

                if (opcode.instruction == Instruction.Add)
                {
                    variables[opcode.arguments[0]] = a + b;
                }
                else if (opcode.instruction == Instruction.Multiply)
                {
                    variables[opcode.arguments[0]] = a * b;
                }
                else if (opcode.instruction == Instruction.Divide)
                {
                    variables[opcode.arguments[0]] = (int)Math.Round((float)a / b, MidpointRounding.ToZero);
                }
                else if (opcode.instruction == Instruction.Modulo)
                {
                    variables[opcode.arguments[0]] = a % b;
                }
                else if (opcode.instruction == Instruction.Equals)
                {
                    variables[opcode.arguments[0]] = a == b ? 1 : 0;
                }
            }

            return variables["z"];
        }

        private static bool IsVariable(string variable)
        {
            return variable is "w" or "x" or "y" or "z";
        }

        private static (Instruction instruction, string[] arguments) ParseInput(string instructions)
        {
            var split = instructions.Split(" ");

            Instruction instruction = split[0] switch
            {
                "inp" => Instruction.Input,
                "add" => Instruction.Add,
                "mul" => Instruction.Multiply,
                "div" => Instruction.Divide,
                "mod" => Instruction.Modulo,
                "eql" => Instruction.Equals,
                _ => Instruction.None
            };

            var arguments = split.Skip(1)
                .ToArray();

            return (instruction, arguments);
        }
    }
}
