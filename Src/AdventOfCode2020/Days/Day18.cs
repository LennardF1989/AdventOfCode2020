using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day18
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day18_Test.txt");
            var lines = File.ReadAllLines("Content\\Day18.txt");

            long sum = 0;
            foreach (var line in lines)
            {
                var expressionLine = line.Replace(" ", string.Empty);

                var answer = CalculateExpression(expressionLine, OperatorPrecedence1);
                Logger.Debug(answer);

                sum += answer;
            }

            Logger.Info($"Day 18A: {sum}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day18_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day18.txt");

            long sum = 0;
            foreach (var line in lines)
            {
                var expressionLine = line.Replace(" ", string.Empty);

                var answer = CalculateExpression(expressionLine, OperatorPrecedence2);
                Logger.Debug(answer);

                sum += answer;
            }

            Logger.Info($"Day 18B: {sum}");
        }

        private static int OperatorPrecedence1(char c)
        {
            if (c == '+' || c == '*')
            {
                return 1;
            }

            return 0;
        }

        private static int OperatorPrecedence2(char c)
        {
            if (c == '+')
            {
                return 2;
            }

            if (c == '*')
            {
                return 1;
            }

            return 0;
        }

        private static long CalculateExpression(string expression, Func<char, int> precedence)
        {
            StringBuilder postfixNotation = new StringBuilder();
            Stack<char> postfixStack = new Stack<char>();

            foreach (var c in expression)
            {
                if (char.IsDigit(c))
                {
                    postfixNotation.Append(c);
                }
                else if (c == '(')
                {
                    postfixStack.Push(c);
                }
                else if (c == ')')
                {
                    while (postfixStack.Count > 0 && postfixStack.Peek() != '(')
                    {
                        postfixNotation.Append(postfixStack.Pop());
                    }

                    postfixStack.TryPop(out _);
                }
                else
                {
                    while (postfixStack.Count > 0 && precedence(c) <= precedence(postfixStack.Peek()))
                    {
                        postfixNotation.Append(postfixStack.Pop());
                    }

                    postfixStack.Push(c);
                }
            }

            while (postfixStack.Count > 0)
            {
                postfixNotation.Append(postfixStack.Pop());
            }

            Stack<long> expressionStack = new Stack<long>();

            foreach (char c in postfixNotation.ToString())
            {
                if (char.IsDigit(c))
                {
                    expressionStack.Push((long) char.GetNumericValue(c));
                }
                else
                {
                    long a = expressionStack.Pop();
                    long b = expressionStack.Pop();

                    if (c == '+')
                    {
                        expressionStack.Push(a + b);
                    }
                    else if (c == '*')
                    {
                        expressionStack.Push(a * b);
                    }
                }
            }

            return expressionStack.Pop();
        }
    }
}
