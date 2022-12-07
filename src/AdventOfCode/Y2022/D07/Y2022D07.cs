using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D07;

public class Y2022D07 : ISolution
{
    [ExpectedResult(1297159, Sample = 95437)]
    public object SolvePartOne(string input)
    {
        return CalcDirSizes(input)
               .Where(kvp => kvp.Value < 100000)
               .Sum(kvp => kvp.Value);
    }

    [ExpectedResult(3866390, Sample = 24933642)]
    public object SolvePartTwo(string input)
    {
        Dictionary<string, int> dirSizes = CalcDirSizes(input);
        int requiredSpace = 30000000 - (70000000 - dirSizes["/"]);
        return dirSizes.OrderBy(kvp => kvp.Value)
                       .First(i => i.Value > requiredSpace)
                       .Value;
    }

    private Dictionary<string, int> CalcDirSizes(string input)
    {
        var lines = InputHelpers.AsLines(input).ToList();
        var dirSizes = new Dictionary<string, int>();
        
        string cdStr = "$ cd ";
        string dirStr = "dir ";

        Stack<string> currDir = new Stack<string>();
        foreach (string line in lines)
        {
            if (line.StartsWith(cdStr))
            {
                string cd = line.Substring(cdStr.Length);
                if (cd == "..")
                {
                    currDir.Pop();
                }
                else
                {
                    currDir.Push(cd);
                }
            }
            else if (line == "$ ls")
            {
                // nothing 
            }
            else if (line.StartsWith(dirStr))
            {
                // nothing
            }
            else
            {
                // File
                int fsize = int.Parse(line.Split(' ')[0]);
                for (int i = 0; i < currDir.Count; i++)
                {
                    string dir = string.Join("_", currDir.Reverse().Take(i + 1));
                    if (!dirSizes.ContainsKey(dir))
                    {
                        dirSizes[dir] = fsize;
                    }
                    else
                    {
                        dirSizes[dir] += fsize;
                    }
                }
            }
        }
        
        return dirSizes;
    }
}