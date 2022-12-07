using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2022.Days
{
    public static class Day07
    {
        class VirtualDirectory
        {
            public Dictionary<string, VirtualDirectory> Directories { get; }
            public Dictionary<string, VirtualFile> Files { get; }

            public string Name { get; }

            public VirtualDirectory(string name)
            {
                Name = name;

                Directories = new Dictionary<string, VirtualDirectory>();
                Files = new Dictionary<string, VirtualFile>();
            }

            public void Traverse(int depth)
            {
                var indentation = string.Join("", Enumerable.Repeat("  ", depth));

                Logger.Debug($"{indentation}- {Name} (dir)");

                foreach (var fsDirectory in Directories.Values)
                {
                    fsDirectory.Traverse(depth + 1);
                }

                foreach (var fsFile in Files.Values)
                {
                    Logger.Debug($"{indentation}  - {fsFile.Name} (file, size={fsFile.Size})");
                }
            }

            public long Size()
            {
                return Files.Values.Sum(x => x.Size) + 
                       Directories.Values.Sum(x => x.Size());
            }
            
            public List<VirtualDirectory> GetAllDirectories()
            {
                var directories = new List<VirtualDirectory> { this };

                foreach (var directory in Directories.Values)
                {
                    directories.AddRange(directory.GetAllDirectories());
                }

                return directories;
            }
        }

        class VirtualFile
        {
            public string Name { get; }
            public int Size { get; }

            public VirtualFile(string name, int size)
            {
                Name = name;
                Size = size;
            }
        }
        
        public static void StartA()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                ;

            var root = GetRoot(lines);
            //root.Traverse(0);

            var answer = root
                .GetAllDirectories()
                .Where(x => x.Size() <= 100000)
                .Sum(x => x.Size());

            Logger.Info($"Day 7A: {answer}");
        }

        public static void StartB()
        {
            var lines = File
                //.ReadAllLines("Content\\Day07_Test.txt")
                .ReadAllLines("Content\\Day07.txt")
                ;

            var root = GetRoot(lines);
            //root.Traverse(0);

            var sizeUsed = root.Size();
            var sizeLeft = 70000000 - sizeUsed;
            var sizeRequired = 30000000 - sizeLeft;

            var answer = root
                .GetAllDirectories()
                .Select(x => x.Size())
                .OrderBy(x => x)
                .FirstOrDefault(directory => directory >= sizeRequired);

            Logger.Info($"Day 7B: {answer}");
        }

        private static VirtualDirectory GetRoot(string[] lines)
        {
            var root = new VirtualDirectory("/");
            var currentPath = new Stack<VirtualDirectory>();
            currentPath.Push(root);

            foreach (var line in lines)
            {
                var tokens = line.Split(" ");
                var currentDirectory = currentPath.Peek();

                if (tokens[0] == "$")
                {
                    if (tokens[1] == "cd")
                    {
                        if (tokens[2] == "/")
                        {
                            currentPath.Clear();
                            currentPath.Push(root);
                        }
                        else if (tokens[2] == "..")
                        {
                            currentPath.Pop();
                        }
                        else
                        {
                            if (!currentDirectory.Directories.TryGetValue(tokens[2], out var newPath))
                            {
                                newPath = new VirtualDirectory(tokens[2]);

                                currentDirectory.Directories.Add(newPath.Name, newPath);
                            }

                            currentPath.Push(newPath);
                        }
                    }
                    else if (tokens[1] == "ls")
                    {
                        //Ignore
                    }

                    continue;
                }

                if (tokens[0] == "dir")
                {
                    if (!currentDirectory.Directories.ContainsKey(tokens[1]))
                    {
                        currentDirectory.Directories.Add(tokens[1], new VirtualDirectory(tokens[1]));
                    }
                }
                else
                {
                    if (!currentDirectory.Files.ContainsKey(tokens[1]))
                    {
                        currentDirectory.Files.Add(tokens[1], new VirtualFile(tokens[1], int.Parse(tokens[0])));
                    }
                }
            }

            return root;
        }
    }
}
