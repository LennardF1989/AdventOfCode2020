using AdventOfCode.Shared;

namespace AdventOfCode2019
{
    public static class IntcodeComputer
    {
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

                Logger.Debug($"Opcode: {fullOpcode}");

                int pm1 = (fullOpcode / 100) % 10;
                int pm2 = (fullOpcode / 1000) % 10;
                int pm3 = (fullOpcode / 10000) % 10;
                int opcode = fullOpcode % 100;

                if (opcode == 1)
                {
                    int p1 = pm1 == 0 ? instructions[instructions[ip + 1]] : instructions[ip + 1];
                    int p2 = pm2 == 0 ? instructions[instructions[ip + 2]] : instructions[ip + 2];
                    int p3 = instructions[ip + 3];

                    instructions[p3] = p1 + p2;

                    ip += 4;
                }
                else if (opcode == 2)
                {
                    int p1 = pm1 == 0 ? instructions[instructions[ip + 1]] : instructions[ip + 1];
                    int p2 = pm2 == 0 ? instructions[instructions[ip + 2]] : instructions[ip + 2];
                    int p3 = instructions[ip + 3];

                    instructions[p3] = p1 * p2;
                    ip += 4;
                }
                else if (opcode == 3)
                {
                    int p1 = instructions[ip + 1];

                    instructions[p1] = input;
                    ip += 2;
                }
                else if (opcode == 4)
                {
                    int p1 = pm1 == 0 ? instructions[instructions[ip + 1]] : instructions[ip + 1];

                    output = instructions[p1];
                    ip += 2;

                    Logger.Debug($"Output: {output}");
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
    }
}
