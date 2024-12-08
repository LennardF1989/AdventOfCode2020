namespace AdventOfCode2024.Days
{
    public class Day05 : IDayA, IDayB
    {
        public void StartA()
        {
            var (
                leftMap,
                rightMap, 
                updatedPages
            ) = ParseInput();
            
            var middlePages = new List<int>();
            
            foreach (var pages in updatedPages)
            {
                var rightOrder = true;

                for (var i = 0; i < pages.Count; i++)
                {
                    var currentPage = pages[i];
                    var rightPages = pages[(i + 1)..].ToHashSet();

                    rightOrder = IsInRightOrder(currentPage, rightPages, leftMap, rightMap);

                    if (!rightOrder)
                    {
                        break;
                    }
                }

                if (rightOrder)
                {
                    middlePages.Add(pages[(int) Math.Floor(pages.Count / 2f)]);
                }
            }

            var answer = middlePages.Sum();

            Logger.Info($"Day 5A: {answer}");
        }
        
        public void StartB()
        {
            var (
                leftMap,
                rightMap, 
                updatedPages
                ) = ParseInput();
            
            var middlePages = new List<int>();
            
            foreach (var pages in updatedPages)
            {
                var rightOrder = true;

                for (var i = 0; i < pages.Count; i++)
                {
                    var currentPage = pages[i];
                    var rightPages = pages[(i + 1)..].ToHashSet();

                    rightOrder = IsInRightOrder(currentPage, rightPages, leftMap, rightMap);

                    if (!rightOrder)
                    {
                        break;
                    }
                }

                if (rightOrder)
                {
                    continue;
                }
                
                var fixedPages = FixOrder(pages, rightMap);
                    
                middlePages.Add(fixedPages[(int) Math.Floor(fixedPages.Count / 2f)]);
            }

            var answer = middlePages.Sum();
            
            Logger.Info($"Day 5B: {answer}");
        }

        private static bool IsInRightOrder(
            int currentPage, 
            HashSet<int> rightPages, 
            Dictionary<int, HashSet<int>> leftMap, 
            Dictionary<int, HashSet<int>> rightMap
        )
        {
            var leftResult = leftMap.GetValueOrDefault(currentPage);

            foreach (var rightPage in rightPages)
            {
                var rightResult = rightMap.GetValueOrDefault(currentPage);

                var left = leftResult != null && leftResult.Contains(rightPage);
                var right = rightResult != null && rightResult.Contains(currentPage);

                if (left || right)
                {
                    continue;
                }

                return false;
            }

            return true;
        }
        
        private static List<int> FixOrder(
            List<int> pages,
            Dictionary<int, HashSet<int>> rightMap
        )
        {
            var newPages = new List<int>
            {
                pages[0]
            };
            
            for (var i = 1; i < pages.Count; i++)
            {
                var currentPage = pages[i];

                var shouldGoBefore = false;
                
                var i2 = 0;
                for (; i2 < newPages.Count; i2++)
                {
                    var newPage = newPages[i2];
                    
                    var pagesBefore = rightMap.GetValueOrDefault(newPage);

                    if (pagesBefore == null || !pagesBefore.Contains(currentPage))
                    {
                        continue;
                    }
                    
                    shouldGoBefore = true;
                    break;
                }

                if (shouldGoBefore)
                {
                    newPages.Insert(i2, currentPage);
                }
                else
                {
                    newPages.Add(currentPage);
                }
            }
            
            return newPages;
        }

        private static (
            Dictionary<int, HashSet<int>> leftMap, 
            Dictionary<int, HashSet<int>> rightMap, 
            List<List<int>> updatedPages
        ) ParseInput()
        {
            var lines = File
                //.ReadAllText("Content\\Day05_Test.txt")
                .ReadAllText("Content\\Day05.txt")
                ;

            var split = lines.Split("\r\n\r\n", true, true);

            var firstHalf = split[0]
                .Split("\n", true, true)
                .SelectList(x =>
                {
                    var result = x.Split("|", true, true);

                    return (left: result[0].ToInteger(), right: result[1].ToInteger());
                });

            var secondHalf = split[1]
                .Split("\r\n", true, true)
                .SelectList(x => x
                    .Split(",", true, true)
                    .SelectList(y => y.ToInteger())
                );
            
            var leftMap = firstHalf
                .GroupBy(x => x.left)
                .ToDictionary(
                    x => x.Key,
                    x => x.SelectHashSet(y => y.right)
                );

            var rightMap = firstHalf
                .GroupBy(x => x.right)
                .ToDictionary(
                    x => x.Key,
                    x => x.SelectHashSet(y => y.left)
                );

            return (leftMap, rightMap, secondHalf);
        }
    }
}