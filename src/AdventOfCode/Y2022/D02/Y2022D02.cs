using System.Diagnostics;
using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D02;

public class Y2022D02 : ISolution
{
    private readonly List<char> _rps = new() { 'A', 'B', 'C' };

    [ExpectedResult(11150, Sample = 15)]
    public object SolvePartOne(string input) =>
        GetRpsGames(input, (_, you) => you switch
            {
                'X' => 'A',
                'Y' => 'B',
                'Z' => 'C',
                _ => throw new ArgumentOutOfRangeException(nameof(you), you, null)
            })
            .Aggregate(0, (agg, game) => agg + GetGameScore(game.opponent, game.you));

    [ExpectedResult(8295, Sample = 12)]
    public object SolvePartTwo(string input) =>
        GetRpsGames(input, (opp, you) => you switch
            {
                'X' => GetLoser(opp),
                'Y' => opp,
                'Z' => GetWinner(opp),
                _ => throw new ArgumentOutOfRangeException(nameof(you), you, null)
            })
            .Aggregate(0, (agg, game) => agg + GetGameScore(game.opponent, game.you));

    private char GetWinner(char selection) => 
        _rps[(_rps.IndexOf(selection) + 1) % _rps.Count];

    private char GetLoser(char selection)
    {
        int idx = _rps.IndexOf(selection) - 1;
        return idx < 0 ? _rps.Last() : _rps[idx];
    }

    private int GetGameScore(char opponent, char you)
    {
        int baseScore = _rps.IndexOf(you) + 1;
        if (you == opponent)
        {
            return baseScore + 3;
        }

        // Use indices as B > A, C > B, A > C.
        bool win = _rps.IndexOf(you) == (_rps.IndexOf(opponent) + 1) % _rps.Count;
        return baseScore + (win ? 6 : 0);
    }

    /// <summary>
    /// Get the rock/paper scissors games from the input.
    /// </summary>
    /// <param name="input">The puzzle input.</param>
    /// <param name="mapYourValue">Allows changing the 'you' result based on the puzzle input. Fn inputs are (opponent, you)</param>
    private IEnumerable<(char opponent, char you)> GetRpsGames(string input, Func<char, char, char> mapYourValue)
    {
        return input.Split('\n')
            .Select(line =>
            {
                string[] split = line.Split(' ');
                char opponent = split[0][0];
                char you = split[1][0];
                return (opponent, mapYourValue(opponent, you));
            });
    }
}