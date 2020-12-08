using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public static class Day8
    {
        enum EOperation
        {
            Accumulator = 0,
            Jump,
            NoOperation
        }

        class Instruction
        {
            public EOperation Operation { get; set; }
            public int IntValue { get; set; }

            public Instruction Clone()
            {
                return new Instruction
                {
                    Operation = Operation,
                    IntValue = IntValue
                };
            }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day8_Test.txt");
            var lines = File.ReadAllLines("Content\\Day8.txt");

            var instructions = ParseLines(lines); 
            int result = InstructionInterpreter(instructions);

            Logger.Info($"Answer 8A: {result}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day8_Test.txt");
            var lines = File.ReadAllLines("Content\\Day8.txt");

            var instructions = ParseLines(lines); 
            int result = InstructionInterpreterFixer(instructions);

            Logger.Info($"Answer 8B: {result}");
        }

        private static List<Instruction> ParseLines(string[] lines)
        {
            var instructions = new List<Instruction>();

            foreach (var line in lines)
            {
                var splittedLine = line.Split(" ");

                var operation = splittedLine[0];
                var argument = splittedLine[1];

                var instruction = new Instruction();

                switch (operation)
                {
                    case "acc":
                        instruction.Operation = EOperation.Accumulator;
                        break;

                    case "jmp":
                        instruction.Operation = EOperation.Jump;
                        break;

                    case "nop":
                        instruction.Operation = EOperation.NoOperation;
                        break;
                }

                instruction.IntValue = int.Parse(argument);

                instructions.Add(instruction);
            }

            return instructions;
        }

        private static int InstructionInterpreter(List<Instruction> originalInstructions)
        {
            int accumulatorValue = 0;
            var instructions = originalInstructions.ToList();

            int currentInstruction = 0;
            Instruction instruction;

            while ((instruction = instructions[currentInstruction]) != null)
            {
                switch (instruction.Operation)
                {
                    case EOperation.Accumulator:
                        accumulatorValue += instruction.IntValue;

                        instructions[currentInstruction] = null;
                        currentInstruction++;
                        break;

                    case EOperation.Jump:
                        instructions[currentInstruction] = null;
                        currentInstruction = (currentInstruction + instruction.IntValue) % instructions.Count;
                        break;

                    case EOperation.NoOperation:
                        instructions[currentInstruction] = null;
                        currentInstruction++;
                        break;
                }
            }

            return accumulatorValue;
        }

        private static int InstructionInterpreterFixer(List<Instruction> originalInstructions)
        {
            var buggedInstructionIndices = originalInstructions
                .Select((x, i) => new { x, i })
                .Where(x => x.x.Operation == EOperation.Jump || x.x.Operation == EOperation.NoOperation)
                .Select(x => x.i)
                .ToList();

            foreach (int buggedInstructionIndex in buggedInstructionIndices)
            {
                var instructions = originalInstructions.ToList();
                instructions[buggedInstructionIndex] = instructions[buggedInstructionIndex].Clone();
                instructions[buggedInstructionIndex].Operation =
                    instructions[buggedInstructionIndex].Operation == EOperation.Jump
                        ? EOperation.NoOperation
                        : EOperation.Jump;

                int accumulatorValue = 0;
                int currentInstruction = 0;
                Instruction instruction;

                while ((instruction = instructions[currentInstruction]) != null)
                {
                    switch (instruction.Operation)
                    {
                        case EOperation.Accumulator:
                            accumulatorValue += instruction.IntValue;

                            instructions[currentInstruction] = null;
                            currentInstruction++;
                            break;

                        case EOperation.Jump:
                            instructions[currentInstruction] = null;
                            currentInstruction = currentInstruction + instruction.IntValue;
                            break;

                        case EOperation.NoOperation:
                            instructions[currentInstruction] = null;
                            currentInstruction++;
                            break;
                    }

                    if (currentInstruction == instructions.Count)
                    {
                        Logger.Debug("Program fixed!");

                        return accumulatorValue;
                    }
                    
                    //NOTE: If not exactly at EOF, then loop around.
                    if (currentInstruction > instructions.Count)
                    {
                        currentInstruction %= instructions.Count;
                    }
                }

                Logger.Debug("Program not fixed, retrying...");
            }

            Logger.Debug("Program not fixed, stopping!");

            return 0;
        }
    }
}
