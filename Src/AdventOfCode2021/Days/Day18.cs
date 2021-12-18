using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode2021.Days
{
    public static class Day18
    {
        public abstract class Node
        {
            public Node Parent { get; set; }

            public abstract string Print(string indent, bool last);
            public abstract Node Clone(Node parent);
        }

        public class PairNode : Node
        {
            public Node Left { get; set; }
            public Node Right { get; set; }

            public PairNode(Node left, Node right)
            {
                Left = left;
                Right = right;
            }

            public override string Print(string indent, bool last)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(indent);

                if (last)
                {
                    stringBuilder.Append("\\-");
                    indent += "  ";
                }
                else
                {
                    stringBuilder.Append("|-");
                    indent += "| ";
                }

                stringBuilder.AppendLine("g");

                indent += "  ";

                stringBuilder.Append(Left.Print(indent, false));
                stringBuilder.Append(Right.Print(indent, true));

                return stringBuilder.ToString();
            }

            public override Node Clone(Node parent)
            {
               var pairNode = new PairNode(null, null)
               {
                   Parent = parent
               };

               pairNode.Left = Left.Clone(pairNode);
               pairNode.Right = Right.Clone(pairNode);

               return pairNode;
            }

            public override string ToString()
            {
                return $"[{Left},{Right}]";
            }
        }

        public class LiteralNode : Node
        {
            public int Value { get; set; }

            public LiteralNode(int value)
            {
                Value = value;
            }

            public override string Print(string indent, bool last)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(indent);
                stringBuilder.Append(last ? "\\-" : "|-");
                stringBuilder.AppendLine(Value.ToString());

                return stringBuilder.ToString();
            }

            public override Node Clone(Node parent)
            {
                return new LiteralNode(Value)
                {
                    Parent = parent
                };
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public static void StartTest()
        {
            var testLines = File.ReadAllLines("Content\\Day18_Test.txt");

            foreach (var line in testLines)
            {
                if (line[0] == ';')
                {
                    continue;
                }

                var splitLine = line.Split("=");

                var expression = GetExpression(splitLine[0]);
                var tree = GetExpressionTree(expression);

                Logger.Debug(tree.ToString() == splitLine[0]);

                Logger.Debug(tree.Print(string.Empty, true));

                (Node, bool) expandedTree;
                while ((expandedTree = ApplySnailfishLogic(tree)).Item2)
                {
                    //Do nothing
                }

                if (splitLine.Length > 1)
                {
                    Logger.Debug(expandedTree.Item1.ToString() == splitLine[1]);
                }
            }
        }

        public static void StartA()
        {
            var lines = File.ReadAllLines("Content\\Day18.txt");
            var trees = new Queue<Node>();

            foreach (var line in lines)
            {
                var expression = GetExpression(line);
                var tree = GetExpressionTree(expression);

                trees.Enqueue(tree);
            }

            var currentTree = trees.Dequeue();

            while (trees.Count > 0)
            {
                var tree = new PairNode(currentTree, trees.Dequeue());
                tree.Left.Parent = tree;
                tree.Right.Parent = tree;

                (Node, bool) expandedTree = (tree, false);
                do
                {
                    expandedTree = ApplySnailfishLogic(expandedTree.Item1);
                } while (expandedTree.Item2);

                currentTree = expandedTree.Item1;
            }

            (Node, bool) magnitudeTree = (currentTree, false);
            do
            {
                magnitudeTree = CalculateMagnitude(magnitudeTree.Item1);
            } while (magnitudeTree.Item2);

            var finalTree = magnitudeTree.Item1;

            int answer = ((LiteralNode)finalTree).Value;

            Logger.Info($"Day 18A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day18_Test2.txt")
                .ReadAllLines("Content\\Day18.txt")
                ;

            var trees = lines
                .Select(GetExpression)
                .Select(GetExpressionTree)
                .ToList();

            var largestMagnitude = 0;

            foreach (var t1 in trees)
            {
                foreach (var t2 in trees)
                {
                    var leftTree = t1.Clone(null);
                    var rightTree = t2.Clone(null);

                    if (leftTree == rightTree)
                    {
                        continue;
                    }

                    var tree = new PairNode(leftTree, rightTree);
                    tree.Left.Parent = tree;
                    tree.Right.Parent = tree;

                    (Node, bool) expandedTree = (tree, false);
                    do
                    {
                        expandedTree = ApplySnailfishLogic(expandedTree.Item1);
                    } while (expandedTree.Item2);
                    
                    (Node, bool) magnitudeTree = (expandedTree.Item1, false);
                    do
                    {
                        magnitudeTree = CalculateMagnitude(magnitudeTree.Item1);
                    } while (magnitudeTree.Item2);

                    var finalTree = magnitudeTree.Item1;

                    var magnitude = ((LiteralNode)finalTree).Value;

                    if (magnitude > largestMagnitude)
                    {
                        largestMagnitude = magnitude;
                    }
                }
            }

            Logger.Info($"Day 18B: {largestMagnitude}");
        }

        private static (Node, bool) ApplySnailfishLogic(Node node)
        {
            var result = FindPairToExplode(node);

            if (result.Item2)
            {
                return result;
            }

            result = FindLiteralToSplit(node);

            if (result.Item2)
            {
                return result;
            }

            return (node, false);
        }

        private static (Node, bool) FindPairToExplode(Node node)
        {
            if (node is not PairNode g)
            {
                return (node, false);
            }

            if (g.Left is LiteralNode ll && g.Right is LiteralNode lr)
            {
                var tempNode = node;
                var count = 0;

                while (tempNode.Parent != null)
                {
                    count++;

                    if (count == 4)
                    {
                        break;
                    }

                    tempNode = tempNode.Parent;
                }

                if (count == 4)
                {
                    //Scan up and take a left
                    {
                        var result = ScanUpForLeft(g);

                        if (result != null)
                        {
                            var literal = ScanDownRight(result);
                            ((LiteralNode)literal).Value += ll.Value;
                        }
                    }

                    //Scan up and take a right
                    {
                        var result = ScanUpForRight(g);

                        if (result != null)
                        {
                            var literal = ScanDownLeft(result);
                            ((LiteralNode)literal).Value += lr.Value;
                        }
                    }

                    var literalNode = new LiteralNode(0)
                    {
                        Parent = g.Parent
                    };

                    return (literalNode, true);
                }
            }

            var newLeft = FindPairToExplode(g.Left);
            g.Left = newLeft.Item1;

            if (newLeft.Item2)
            {
                return (node, true);
            }

            var newRight = FindPairToExplode(g.Right);
            g.Right = newRight.Item1;

            if (newRight.Item2)
            {
                return (node, true);
            }

            return (node, false);
        }

        private static (Node, bool) FindLiteralToSplit(Node node)
        {
            if (node is PairNode g)
            {
                var newLeft = FindLiteralToSplit(g.Left);
                g.Left = newLeft.Item1;

                if (newLeft.Item2)
                {
                    return (node, true);
                }

                var newRight = FindLiteralToSplit(g.Right);
                g.Right = newRight.Item1;

                if (newRight.Item2)
                {
                    return (node, true);
                }
            }
            else if (node is LiteralNode l && l.Value >= 10)
            {
                int left = (int)Math.Floor(l.Value / 2f);
                int right = (int)Math.Ceiling(l.Value / 2f);

                var groupNode = new PairNode(
                    new LiteralNode(left),
                    new LiteralNode(right)
                );

                groupNode.Parent = node.Parent;
                groupNode.Left.Parent = groupNode;
                groupNode.Right.Parent = groupNode;

                return (groupNode, true);
            }

            return (node, false);
        }

        private static (Node, bool) CalculateMagnitude(Node node)
        {
            if (node is not PairNode g)
            {
                return (node, false);
            }

            if (g.Left is LiteralNode ll && g.Right is LiteralNode lr)
            {
                var literalNode = new LiteralNode(ll.Value * 3 + lr.Value * 2)
                {
                    Parent = g.Parent
                };

                return (literalNode, true);
            }

            var newLeft = CalculateMagnitude(g.Left);
            g.Left = newLeft.Item1;

            if (newLeft.Item2)
            {
                return (node, true);
            }

            var newRight = CalculateMagnitude(g.Right);
            g.Right = newRight.Item1;

            if (newRight.Item2)
            {
                return (node, true);
            }

            return (node, false);
        }

        private static Node ScanUpForLeft(Node currentNode)
        {
            var parentNode = currentNode.Parent;

            if (parentNode == null)
            {
                return null;
            }

            var leftNode = ((PairNode)parentNode).Left;

            if (leftNode != currentNode)
            {
                return leftNode;
            }

            return ScanUpForLeft(parentNode);
        }

        private static Node ScanUpForRight(Node currentNode)
        {
            var parentNode = currentNode.Parent;

            if (parentNode == null)
            {
                return null;
            }

            var rightNode = ((PairNode)parentNode).Right;

            if (rightNode != currentNode)
            {
                return rightNode;
            }

            return ScanUpForRight(parentNode);
        }

        private static Node ScanDownLeft(Node currentNode)
        {
            if (currentNode is LiteralNode)
            {
                return currentNode;
            }

            return ScanDownLeft(((PairNode)currentNode).Left);
        }

        private static Node ScanDownRight(Node currentNode)
        {
            if (currentNode is LiteralNode)
            {
                return currentNode;
            }

            return ScanDownRight(((PairNode)currentNode).Right);
        }

        private static Node GetExpressionTree(string expression)
        {
            Stack<Node> expressionStack = new Stack<Node>();

            foreach (char c in expression)
            {
                if (char.IsDigit(c))
                {
                    expressionStack.Push(new LiteralNode((int)char.GetNumericValue(c)));
                }
                else
                {
                    Node a = expressionStack.Pop();
                    Node b = expressionStack.Pop();

                    if (c == ',')
                    {
                        var groupNode = new PairNode(b, a);
                        groupNode.Left.Parent = groupNode;
                        groupNode.Right.Parent = groupNode;

                        expressionStack.Push(groupNode);
                    }
                }
            }

            return expressionStack.Pop();
        }

        private static string GetExpression(string line)
        {
            StringBuilder postfixNotation = new StringBuilder();
            Stack<char> postfixStack = new Stack<char>();

            foreach (var c in line)
            {
                if (char.IsDigit(c))
                {
                    postfixNotation.Append(c);
                }
                else if (c == '[')
                {
                    postfixStack.Push(c);
                }
                else if (c == ']')
                {
                    while (postfixStack.Count > 0 && postfixStack.Peek() != '[')
                    {
                        postfixNotation.Append(postfixStack.Pop());
                    }

                    postfixStack.TryPop(out _);
                }
                else
                {
                    while (postfixStack.Count > 0 && OperatorPrecedence(c) <= OperatorPrecedence(postfixStack.Peek()))
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

            return postfixNotation.ToString();
        }

        private static int OperatorPrecedence(char c)
        {
            return c == ',' ? 1 : 0;
        }
    }
}
