using System.Text.RegularExpressions;
using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D02;

public class Y2023D02 : ISolution
{
    record Round(int Red, int Green, int Blue);

    record Game(int Id, Round[] Rounds);

    [ExpectedResult(2076, Sample = 8)]
    public object SolvePartOne(string input)
    {
        const int rLim = 12;
        const int gLim = 13;
        const int bLim = 14;

        return InputHelpers.AsLines(input)
            .Select(line =>
            {
                var split = line.Split(':');
                var gameId = int.Parse(split[0].Replace("Game ", ""));

                var rounds = split[1]
                    .Split(';') // rounds
                    .Select(ParseRoundResult)
                    .ToArray();

                var ret = new Game(gameId, rounds);
                return ret;
            })
            .Where(g =>
                g.Rounds.All(r => r.Red <= rLim) &&
                g.Rounds.All(r => r.Green <= gLim) &&
                g.Rounds.All(r => r.Blue <= bLim))
            .Sum(g => g.Id);
    }

    [ExpectedResult(70950, Sample = 2286)]
    public object SolvePartTwo(string input)
    {
        return InputHelpers.AsLines(input)
            .Select(line =>
            {
                var split = line.Split(':');
                var gameId = int.Parse(split[0].Split(' ')[1]);

                var rounds = split[1]
                    .Split(';') // rounds
                    .Select(ParseRoundResult)
                    .ToArray();

                var ret = new Game(gameId, rounds);
                return ret;
            })
            .Select(g =>
                g.Rounds.Max(r => r.Red) *
                g.Rounds.Max(r => r.Green) *
                g.Rounds.Max(r => r.Blue))
            .Sum();
    }

    private Round ParseRoundResult(string round)
    {
        var matches = Regex.Matches(round, @"(\d+) (\w+)");

        return new Round(
            int.Parse(matches.FirstOrDefault(m => m.Groups[2].Value == "red")?.Groups[1].Value ?? "0"),
            int.Parse(matches.FirstOrDefault(m => m.Groups[2].Value == "green")?.Groups[1].Value ?? "0"),
            int.Parse(matches.FirstOrDefault(m => m.Groups[2].Value == "blue")?.Groups[1].Value ?? "0"));
    }
}