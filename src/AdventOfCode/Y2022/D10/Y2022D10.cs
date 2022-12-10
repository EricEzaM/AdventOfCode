using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

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
            int cycles = _cycles[op];
            for (int i = 0; i < cycles; i++)
            {
                cyclesComplete++;

                if (cyclesComplete == 20 || (cyclesComplete - 20) % 40 == 0)
                {
                    str += x * cyclesComplete;
                }
            }

            if (op == "addx")
            {
                x += val;
            }
        }

        return str;
    }

    // ZFBFHGUP
    private const string P2Expected = @"####.####.###..####.#..#..##..#..#.###..
...#.#....#..#.#....#..#.#..#.#..#.#..#.
..#..###..###..###..####.#....#..#.#..#.
.#...#....#..#.#....#..#.#.##.#..#.###..
#....#....#..#.#....#..#.#..#.#..#.#....
####.#....###..#....#..#..###..##..#....";
    
    private const string P2ExpectedSample = @"##..##..##..##..##..##..##..##..##..##..
###...###...###...###...###...###...###.
####....####....####....####....####....
#####.....#####.....#####.....#####.....
######......######......######......####
#######.......#######.......#######.....";
    
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
            int cycles = _cycles[op];
            for (int i = 0; i < cycles; i++)
            {
                var crtRowPos = crtPos % 40;
                crtImage[crtPos] = crtRowPos >= sprPos && crtRowPos <= sprPos + 2 ? '#' : '.';
                crtPos++;
            }

            if (op == "addx")
            {
                sprPos += val;
            }
        }

        return string.Join("\n", crtImage.Chunk(40).Select(ca => new string(ca)));
    }
}