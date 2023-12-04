using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D04;

public class Y2023D04 : ISolution
{
    record Card(List<int> Winners, List<int> Numbers)
    {
        public int GetWinCount() => Winners.Intersect(Numbers).Count();
    };

    [ExpectedResult(26914, Sample = 13)]
    public object SolvePartOne(string input)
    {
        List<Card> cards = ParseCards(input);

        return (int)cards
            .Select(c => c.GetWinCount())
            .Select(n => n > 0 ? Math.Pow(2, n - 1) : 0)
            .Sum();
    }

    [ExpectedResult(13080971, Sample = 30)]
    public object SolvePartTwo(string input)
    {
        List<Card> cards = ParseCards(input);
        // Array to track how many of each card we have
        int[] cardMultiples = Enumerable.Repeat(1, cards.Count).ToArray();

        for (var cardIdx = 0; cardIdx < cards.Count; cardIdx++)
        {
            Card card = cards[cardIdx];
            int winCount = card.GetWinCount();

            for (int duplicateCardIdx = cardIdx + 1; // Start at the next card in the card list
                 duplicateCardIdx < cardIdx + winCount + 1 && duplicateCardIdx < cardMultiples.Length; // Loop until the end of the array or until the end of the number of won cards 
                 duplicateCardIdx++)
            {
                cardMultiples[duplicateCardIdx] += cardMultiples[cardIdx]; // We win cards equal to the current card count
            }
        }

        return cardMultiples.Sum();
    }

    private List<Card> ParseCards(string input)
    {
        return InputHelpers.AsLines(input)
            .Select(line => line.Split(':')[1].Split('|'))
            .Select(arr => (winners: arr[0], numebers: arr[1]))
            .Select(t =>
                new Card(t.winners.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList(),
                    t.numebers.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList()))
            .ToList();
    }
}