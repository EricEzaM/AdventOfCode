using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D05;

public class Y2022D05 : ISolution
{
    [ExpectedResult("MQTPGLLDN", Sample = "CMZ")]
    public object SolvePartOne(string input)
    {
        var stacks = ParseStartStacks(input);
        var movements = ParseMovements(input);

        foreach ((int count, int fromIdx, int toIdx) in movements)
        {
            for (int i = 0; i < count; i++)
            {
                if (stacks[fromIdx].TryPop(out string? popped))
                {
                    stacks[toIdx].Push(popped);
                }
            }
        }
        
        return string.Join("", stacks.Select(s => s.Peek().Trim('[', ']')));
    }

    [ExpectedResult("LVZPSTTCZ", Sample = "MCD")]
    public object SolvePartTwo(string input)
    {
        var stacks = ParseStartStacks(input);
        var movements = ParseMovements(input);
        
        // A bit hacky, just to keep using the stacks :)
        List<string> toPush = new();
        foreach ((int count, int fromIdx, int toIdx) in movements)
        {
            for (int i = 0; i < count; i++)
            {
                if (stacks[fromIdx].TryPop(out string? popped))
                {
                    toPush.Add(popped);
                }
            }

            foreach (var c in toPush.AsEnumerable().Reverse())
            {
                stacks[toIdx].Push(c);
            }
            
            toPush.Clear();
        }
        
        return string.Join("", stacks.Select(s => s.Peek().Trim('[', ']')));

    }

    private List<Stack<string>> ParseStartStacks(string input)
    {
        string[] split = input.Split("\n\n");
        int stackCount = InputHelpers.AsLines(split[0])
                                     .Last()
                                     .Split(' ')
                                     .Count(s => !string.IsNullOrEmpty(s));

        return InputHelpers.AsLines(split[0])
                           .Select(l => l.Chunk(4).Select(cs => new string(cs).Trim()))
                           .SkipLast(1)
                           .Reverse()
                           .SelectMany(crate => crate)
                           .Select((crate, index) => (crate, index))
                           .Aggregate(CreateStacks(stackCount).ToList(), (list, c) =>
                           {
                               if (!string.IsNullOrWhiteSpace(c.crate))
                               {
                                   list[c.index % stackCount].Push(c.crate);
                               }

                               return list;
                           });
    }

    private List<(int count, int fromIdx, int toIdx)> ParseMovements(string input)
    {
        string[] split = input.Split("\n\n");
        return InputHelpers.AsLines(split[1])
                           .Select(l => l.Split(' '))
                           .Select(arr => (int.Parse(arr[1]), int.Parse(arr[3]) - 1, int.Parse(arr[5]) - 1))
                           .ToList();
    }
    
    private IEnumerable<Stack<string>> CreateStacks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new Stack<string>();
        }
    }
}