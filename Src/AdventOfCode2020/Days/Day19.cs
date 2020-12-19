using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day19
    {
        public abstract class Node
        {
            public List<Node> Nodes { get; set; }

            protected Node()
            {
                Nodes = new List<Node>();
            }

            public abstract bool ValidateLine(string line, ref int index);

            public virtual string GetRegex()
            {
                return string.Join(string.Empty, Nodes.Select(x => x.GetRegex()));
            }

            public override string ToString()
            {
                return "Node";
            }
        }

        //NOTE: A rule-node only has a single child node
        public class RuleNode : Node
        {
            public string Name { get; set; }
            public string Rule { get; set; }
            
            public override bool ValidateLine(string line, ref int index)
            {
                int localIndex = index;

                if (Nodes[0].ValidateLine(line, ref localIndex))
                {
                    index = localIndex;

                    return true;
                }

                return false;
            }

            public override string ToString()
            {
                return $"R:{Name}";
            }
        }

        //NOTE: A sequence-node needs all of its child nodes to be true
        public class SequenceNode : Node
        {
            public override bool ValidateLine(string line, ref int index)
            {
                int localIndex = index;

                foreach (var node in Nodes)
                {
                    //NOTE: If the sequence has nodes remaining, but the line is done, this is a false.
                    if (localIndex >= line.Length)
                    {
                        return false;
                    }

                    if (!node.ValidateLine(line, ref localIndex))
                    {
                        return false;
                    }
                }

                index = localIndex;

                return true;
            }

            public override string ToString()
            {
                return $"S:({string.Join(" ", Nodes.Select(x => x.ToString()))})";
            }
        }

        //NOTE: An or-node needs any of the child nodes to be true
        public class OrNode : Node
        {
            public string RecursiveRule { get; set; }

            public override bool ValidateLine(string line, ref int index)
            {
                foreach (var node in Nodes)
                {
                    //NOTE: Reset the localIndex on every attempt
                    int localIndex = index;

                    if (node.ValidateLine(line, ref localIndex))
                    {
                        index = localIndex;

                        return true;
                    }
                }

                return false;
            }

            public override string GetRegex()
            {
                //Repeat 42 to a max of 10 times
                if (RecursiveRule == "8")
                {
                    var list = new List<string>();

                    for (int i = 1; i <= 10; i++)
                    {
                        list.Add("(" + string.Join(string.Empty, Enumerable.Repeat(Nodes[0].GetRegex(), i)) + ")");
                    }

                    return "(" + string.Join("|", list) + ")";
                }

                //Repeat 42 .. 32 to a max of 10 times
                if (RecursiveRule == "11")
                {
                    var list = new List<string>();

                    for (int i = 1; i <= 10; i++)
                    {
                        string result = "(" + string.Join(string.Empty, Enumerable.Repeat(Nodes[0].Nodes[0].GetRegex(), i)) + ")";
                        result += "(" + string.Join(string.Empty, Enumerable.Repeat(Nodes[0].Nodes[1].GetRegex(), i)) + ")";

                        list.Add($"({result})");
                    }

                    return "(" + string.Join("|", list) + ")";
                }

                return "(" + string.Join("|", Nodes.Select(x => x.GetRegex())) + ")";
            }

            public override string ToString()
            {
                return $"OR:({string.Join("|", Nodes.Select(x => x.ToString()))})";
            }
        }

        //NOTE: A literal-node is finite and is also in charge of incrementing the index
        public class LiteralNode : Node
        {
            public char Literal { get; set; }

            public LiteralNode(char literal)
            {
                Literal = literal;
            }

            public override bool ValidateLine(string line, ref int index)
            {
                if (line[index] == Literal)
                {
                    index++;

                    return true;
                }

                return false;
            }

            public override string GetRegex()
            {
                return Literal.ToString();
            }

            public override string ToString()
            {
                return "L:" + GetRegex();
            }
        }

        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day19_Test.txt");
            //var lines = File.ReadAllLines("Content\\Day19_Test2.txt");
            var lines = File.ReadAllLines("Content\\Day19.txt");

            var rootNode = ParseRules(lines, false);
            var regex = $"^{rootNode.GetRegex()}$";

            var linesToCheck = lines.Where(x => !x.Contains(":") && !string.IsNullOrWhiteSpace(x));

            int matchCount1 = 0;
            int matchCount2 = 0;

            foreach (var line in linesToCheck)
            {
                Logger.Debug(line);

                if (Regex.IsMatch(line, regex))
                {
                    matchCount1++;
                }

                int index = 0;
                if (rootNode.ValidateLine(line, ref index) && index == line.Length)
                {
                    matchCount2++;
                }
            }

            Logger.Debug($"ValidateLine: {matchCount2}");

            Logger.Info($"Day 19A: {matchCount1}");
        }

        public static void StartB()
        {
            //var lines = File.ReadAllLines("Content\\Day19_Test2B.txt");
            var lines = File.ReadAllLines("Content\\Day19B.txt");

            var rootNode = ParseRules(lines, true);
            var regex = $"^{rootNode.GetRegex()}$";

            var linesToCheck = lines.Where(x => !x.Contains(":") && !string.IsNullOrWhiteSpace(x));

            int matchCount1 = 0;

            foreach (var line in linesToCheck)
            {
                Logger.Debug(line);

                if (Regex.IsMatch(line, regex))
                {
                    matchCount1++;
                }
            }

            Logger.Info($"Day 19B: {matchCount1}");
        }

        private static RuleNode ParseRules(string[] lines, bool applyHack)
        {
            List<string> rules = lines
                .Where(x => x.Contains(":"))
                .ToList();

            List<RuleNode> ruleNodes = rules
                .Select(rule => rule.Split(":"))
                .Select(rule1 => new RuleNode
                {
                    Name = rule1[0], 
                    Rule = rule1[1]
                })
                .ToList();

            foreach (var ruleNode in ruleNodes)
            {
                var orRules = ruleNode.Rule.Split("|");

                if (orRules.Length > 1)
                {
                    List<Node> nodes = new List<Node>();

                    foreach (var rule in orRules)
                    {
                        nodes.Add(GetSequenceRule(ruleNodes, rule));
                    }

                    ruleNode.Nodes.Add(new OrNode
                    {
                        RecursiveRule = (applyHack && (ruleNode.Name == "8" || ruleNode.Name == "11")) ? ruleNode.Name : null, //HACK: Recursive for 19B
                        Nodes = nodes
                    });
                }
                else
                {
                    var node = GetSequenceRule(ruleNodes, orRules[0]);

                    ruleNode.Nodes.Add(node);
                }
            }

            return ruleNodes.First(x => x.Name == "0");
        }

        private static SequenceNode GetSequenceRule(List<RuleNode> ruleNodes, string line)
        {
            var splitLine = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var nodes = new List<Node>();

            foreach (var node in splitLine)
            {
                if (node.StartsWith("\""))
                {
                    //Literal
                    nodes.Add(new LiteralNode(node.Trim('"')[0]));
                }
                else
                {
                    //Rule
                    nodes.Add(ruleNodes.First(x => x.Name == node));
                }
            }

            return new SequenceNode
            {
                Nodes = nodes
            };
        }
    }
}
