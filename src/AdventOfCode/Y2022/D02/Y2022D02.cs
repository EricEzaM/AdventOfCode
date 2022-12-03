using AdventOfCode.Lib;

namespace AdventOfCode.Y2022.D02;

public class Y2022D02 : ISolution
{
    private const string Rock = "R";
    private const string Paper = "P";
    private const string Scissors = "S";

    private readonly Dictionary<string, int> _selectionScores = new()
    {
        { Rock, 1 },
        { Paper, 2 },
        { Scissors, 3 },
    };

    private readonly Dictionary<string, string> _selectionMap = new()
    {
        { "X", Rock },
        { "Y", Paper },
        { "Z", Scissors },
        { "A", Rock },
        { "B", Paper },
        { "C", Scissors },
    };

    private readonly Dictionary<string, string> _winConds = new()
    {
        { Rock, Paper },
        { Paper, Scissors },
        { Scissors, Rock }
    };
    
    public object SolvePartOne(string input)
    {
        int totalScore = GetRpsGames(input)
            .Aggregate(0, (agg, game) => agg + CalculateGameScore(game.opponent, game.you));
        
        return totalScore;
    }

    public object SolvePartTwo(string input)
    {
        return string.Empty;
    }

    private int CalculateGameScore(string opponent, string you)
    {
        int baseScore = _selectionScores[you];
        if (_winConds[opponent] == you)
        {
            return baseScore + 6;
        }

        if (opponent == you)
        {
            return baseScore + 3;
        }

        return baseScore;
    }
    
    private IEnumerable<(string opponent, string you)> GetRpsGames(string input)
    {
        return input.Split('\n')
            .Select(line =>
            {
                string[] split = line.Split(' ');
                return (_selectionMap[split[0]], _selectionMap[split[1]]);
            });
    }
}