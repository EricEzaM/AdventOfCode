using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2022.D10;

public class Y2022D10 : ISolution
{
    private readonly Dictionary<string, int> _cycles = new()
    {
        {"noop", 1},
        {"addx", 2}
    };

    [ExpectedResult(15680, Sample = 13140)]
    public object SolvePartOne(string input)
    {
        var commands = InputHelpers.AsLines(input)
                                   .Select(l => l.Split(' '))
                                   .Select(s => (op: s[0], val: s.Length > 1 ? int.Parse(s[1]) : 0));

        int cyclesComplete = 0;
        int x = 1;
        int str = 0;

        foreach ((string op, int val) in commands)
        {
            for (int i = 0; i < _cycles[op]; i++)
            {
                cyclesComplete++;
                str += cyclesComplete == 20 || (cyclesComplete - 20) % 40 == 0 ? x * cyclesComplete : 0;
            }

            x += op == "addx" ? val : 0;
        }

        return str;
    }

    // ZFBFHGUP
    private const string P2Expected =
        "####.####.###..####.#..#..##..#..#.###..\n...#.#....#..#.#....#..#.#..#.#..#.#..#.\n..#..###..###..###..####.#....#..#.#..#.\n.#...#....#..#.#....#..#.#.##.#..#.###..\n#....#....#..#.#....#..#.#..#.#..#.#....\n####.#....###..#....#..#..###..##..#....";

    private const string P2ExpectedSample =
        "##..##..##..##..##..##..##..##..##..##..\n###...###...###...###...###...###...###.\n####....####....####....####....####....\n#####.....#####.....#####.....#####.....\n######......######......######......####\n#######.......#######.......#######.....";

    [ExpectedResult(P2Expected, Sample = P2ExpectedSample)]
    public object SolvePartTwo(string input)
    {
        var commands = InputHelpers.AsLines(input)
                                   .Select(l => l.Split(' '))
                                   .Select(s => (op: s[0], val: s.Length > 1 ? int.Parse(s[1]) : 0));

        int crtPos = 0;
        int sprPos = 0;

        char[] crtImage = new char[240];

        foreach ((string op, int val) in commands)
        {
            for (int i = 0; i < _cycles[op]; i++)
            {
                var crtRowPos = crtPos % 40;
                crtImage[crtPos] = crtRowPos >= sprPos && crtRowPos <= sprPos + 2 ? '#' : '.';
                crtPos++;
            }

            sprPos += op == "addx" ? val : 0;
        }

        return string.Join("\n", crtImage.Chunk(40).Select(ca => new string(ca)));
    }
}