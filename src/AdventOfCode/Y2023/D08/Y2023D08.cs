using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D08;

public class Y2023D08 : ISolution
{
    private record struct NodeVisit(Node Node, char NextStep);

    private record Node(string Id, string Left, string Right);

    [ExpectedResult(16409, Sample = 6)]
    public object SolvePartOne(string input)
    {
        Parse(input, out Dictionary<string, Node> nodes, out char[] steps);

        int moveCount = 0;
        char step = steps.First();
        string currentNode = "AAA";
        while (currentNode != "ZZZ")
        {
            moveCount++;
            currentNode = step == 'L' ? nodes[currentNode].Left : nodes[currentNode].Right;
            step = steps[moveCount % steps.Length];
        }

        return moveCount;
    }

    [ExpectedResult(11795205644011, Sample = 6)]
    public object SolvePartTwo(string input)
    {
        Parse(input, out Dictionary<string, Node> nodes, out char[] steps);

        // Find the first index for each starting node where Z occurs.
        // Then get the lowest common multiple of those.
        List<string> currentNodes = nodes.Keys.Where(k => k[2] == 'A').ToList();
        ulong[] zFinds = Enumerable.Repeat(0UL, currentNodes.Count).ToArray();
        
        ulong moveCount = 0;
        char step = steps.First();
        ulong stepCount = (ulong)steps.Length;
        
        while (true)
        {
            moveCount++;
        
            for (int i = 0; i < currentNodes.Count; i++)
            {
                if (zFinds[i] != 0)
                {
                    continue;
                }
                
                string currentNode = currentNodes[i];
                currentNodes[i] = step == 'L' ? nodes[currentNode].Left : nodes[currentNode].Right;

                if (currentNodes[i][2] == 'Z')
                {
                    zFinds[i] = moveCount;
                }
            }

            if (zFinds.All(i => i != 0))
            {
                break;
            }
        
            step = steps[moveCount % stepCount];
        }

        return LowestCommonMultiple(zFinds);
    }

    private void Parse(string input, out Dictionary<string, Node> nodes, out char[] steps)
    {
        input = input
            .Replace(" = ", " ")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(",", "");

        string[] lines = InputHelpers.AsLines(input);
        steps = lines.First().ToCharArray();


        nodes = lines.Skip(2)
            .Select(l => l.Split(' '))
            .ToDictionary(arr => arr[0], arr => new Node(arr[0], arr[1], arr[2]));
    }

    static ulong GreatestCommonFactor(ulong a, ulong b)
    {
        while (b != 0)
        {
            ulong temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    static ulong LowestCommonMultiple(ulong a, ulong b)
    {
        return a / GreatestCommonFactor(a, b) * b;
    }

    static ulong LowestCommonMultiple(ulong[] ints)
    {
        return ints.Aggregate(1UL, LowestCommonMultiple);
    }
}