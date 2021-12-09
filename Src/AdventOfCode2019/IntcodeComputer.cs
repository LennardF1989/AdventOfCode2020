using AdventOfCode.Shared;

namespace AdventOfCode2019
{
    public static class IntcodeComputer
    {
        private const int POSITION_MODE = 0;
        private const int IMMEDIATE_MODE = 1;

        //Day 2
        public static void RunCode(int[] instructions)
        {
            RunCode(instructions, 0, out _);
        }

        //Day 5
        public static void RunCode(int[] instructions, int input, out int output)
        {
            output = 0;

            for (var ip = 0; ip < instructions.Length;)
            {
                var fullOpcode = instructions[ip];

                int pm1 = (fullOpcode / 100) % 10;
                int pm2 = (fullOpcode / 1000) % 10;
                int pm3 = (fullOpcode / 10000) % 10;
                int opcode = fullOpcode % 100;

                if (opcode == 1)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);
                    int p2 = GetParameter(instructions, ip, pm2, 2);
                    int p3 = GetParameter(instructions, ip, IMMEDIATE_MODE, 3);

                    instructions[p3] = p1 + p2;

                    ip += 4;
                }
                else if (opcode == 2)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);
                    int p2 = GetParameter(instructions, ip, pm2, 2);
                    int p3 = GetParameter(instructions, ip, IMMEDIATE_MODE, 3);

                    instructions[p3] = p1 * p2;
                    ip += 4;
                }
                else if (opcode == 3)
                {
                    int p1 = GetParameter(instructions, ip, IMMEDIATE_MODE, 1);

                    instructions[p1] = input;
                    ip += 2;
                }
                else if (opcode == 4)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);

                    output = p1;
                    ip += 2;

                    Logger.Debug($"Output: {output}");
                }
                else if (opcode == 5)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);

                    if (p1 != 0)
                    {
                        int p2 = GetParameter(instructions, ip, pm2, 2);

                        ip = p2;
                    }
                    else
                    {
                        ip += 3;
                    }
                }
                else if (opcode == 6)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);

                    if (p1 == 0)
                    {
                        int p2 = GetParameter(instructions, ip, pm2, 2);

                        ip = p2;
                    }
                    else
                    {
                        ip += 3;
                    }
                }
                else if (opcode == 7)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);
                    int p2 = GetParameter(instructions, ip, pm2, 2);
                    int p3 = GetParameter(instructions, ip, IMMEDIATE_MODE, 3);

                    if (p1 < p2)
                    {
                        instructions[p3] = 1;
                    }
                    else
                    {
                        instructions[p3] = 0;
                    }

                    ip += 4;
                }
                else if (opcode == 8)
                {
                    int p1 = GetParameter(instructions, ip, pm1, 1);
                    int p2 = GetParameter(instructions, ip, pm2, 2);
                    int p3 = GetParameter(instructions, ip, IMMEDIATE_MODE, 3);

                    if (p1 == p2)
                    {
                        instructions[p3] = 1;
                    }
                    else
                    {
                        instructions[p3] = 0;
                    }

                    ip += 4;
                }
                else if (opcode == 99)
                {
                    break;
                }
                else
                {
                    Logger.Debug("PANIC!");

                    break;
                }
            }
        }

        private static int GetParameter(int[] instructions, int instructionPointer, int parameterMode, int index)
        {
            var instruction = instructions[instructionPointer + index];

            return parameterMode == POSITION_MODE 
                ? instructions[instruction] 
                : instruction;
        }
    }
}
