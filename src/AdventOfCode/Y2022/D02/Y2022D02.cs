using AdventOfCode.Lib;

namespace AdventOfCode.Y2022.D02;

public class Y2022D02 : ISolution
{
    private readonly List<string> _rps = new() { "A", "B", "C" };

    public object SolvePartOne(string input) =>
        GetRpsGames(input, (_, you) => you switch
            {
                "X" => "A",
                "Y" => "B",
                "Z" => "C",
                _ => throw new ArgumentOutOfRangeException(nameof(you), you, null)
            })
            .Aggregate(0, (agg, game) => agg + GetGameScore(game.opponent, game.you));

    public object SolvePartTwo(string input) =>
        GetRpsGames(input, (opp, you) => you switch
            {
                "X" => GetPrevious(opp), // Must lose
                "Y" => opp, // Must Draw
                "Z" => GetNext(opp), // Must Win
                _ => throw new ArgumentOutOfRangeException(nameof(you), you, null)
            })
            .Aggregate(0, (agg, game) => agg + GetGameScore(game.opponent, game.you));

    /// <summary>
    /// Get next R/P/S selection from the list, after selection.
    /// </summary>
    private string GetNext(string selection) => 
        _rps[(_rps.IndexOf(selection) + 1) % _rps.Count];

    /// <summary>
    /// Get previous R/P/S selection from the list, after selection.
    /// </summary>
    private string GetPrevious(string selection)
    {
        int idx = _rps.IndexOf(selection) - 1;
        return idx < 0 ? _rps.Last() : _rps[idx];
    }

    private int GetGameScore(string opponent, string you)
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
    /// <param name="mapOppYou">Allows changing the 'you' result based on the puzzle input.</param>
    private IEnumerable<(string opponent, string you)> GetRpsGames(string input, Func<string, string, string> mapOppYou)
    {
        return input.Split('\n')
            .Select(line =>
            {
                string[] split = line.Split(' ');
                return (split[0], mapOppYou(split[0], split[1]));
            });
    }
}