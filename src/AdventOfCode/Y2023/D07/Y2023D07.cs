using System.Diagnostics;
using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D07;

public class Y2023D07 : ISolution
{
    private static readonly List<char> _cardOrder = new()
    {
        '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
    };

    private class Hand
    {
        private readonly string _cardsString;

        public class Comparer : IComparer<Hand>
        {
            public int Compare(Hand? x, Hand? y)
            {
                if (x!.HandType != y!.HandType)
                {
                    return x.HandType - y.HandType;
                }

                var mismatchPair = x.Cards
                    .Zip(y.Cards)
                    .First(c => c.First != c.Second);

                return _cardOrder.IndexOf(mismatchPair.First) - _cardOrder.IndexOf(mismatchPair.Second);
            }
        }

        public enum EHandType
        {
            High,
            OneP,
            TwoP,
            ThreeKind,
            FullHouse,
            FourKind,
            FiveKind,
        }

        public EHandType HandType { get; }

        public Hand(string cardsString, int bid)
        {
            _cardsString = cardsString;
            Cards = cardsString.ToCharArray();
            Bid = bid;

            List<int> charCounts = cardsString.GroupBy(c => c)
                .Select(g => g.Count())
                .OrderByDescending(c => c)
                .ToList();

            HandType = charCounts switch
            {
                [1, ..] => EHandType.High,
                [2, 1, ..] => EHandType.OneP,
                [2, 2, ..] => EHandType.TwoP,
                [3, 1, ..] => EHandType.ThreeKind,
                [3, 2, ..] => EHandType.FullHouse,
                [4, ..] => EHandType.FourKind,
                [5, ..] => EHandType.FiveKind,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public char[] Cards { get; init; }
        public int Bid { get; init; }
    }

    [ExpectedResult("", Sample = 6440)]
    public object SolvePartOne(string input)
    {
        Hand.Comparer comparer = new();

        var list = InputHelpers.AsLines(input)
            .Select(l =>
            {
                string[] split = l.Split(' ');
                return new Hand(split[0], int.Parse(split[1]));
            })
            .OrderBy(h => h, comparer)
            .ToList();

        return list
            .Select((hand, idx) => (hand, idx))
            .Aggregate(0, (total, tuple) => total + tuple.hand.Bid * (tuple.idx + 1));
    }

    [ExpectedResult("", Sample = "")]
    public object SolvePartTwo(string input)
    {
        return string.Empty;
    }
}