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

    private static readonly List<char> _cardOrderJokers = new()
    {
        'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A',
    };

    private class Hand
    {
        public class Comparer : IComparer<Hand>
        {
            private readonly bool _useJoker;

            public Comparer(bool useJoker)
            {
                _useJoker = useJoker;
            }

            public int Compare(Hand? x, Hand? y)
            {
                if (x!.HandType != y!.HandType)
                {
                    return x.HandType - y.HandType;
                }

                if (x.CardsString == y.CardsString)
                {
                    return 0;
                }

                var mismatchPair = x.Cards
                    .Zip(y.Cards)
                    .First(c => c.First != c.Second);

                List<char> order = _useJoker ? _cardOrderJokers : _cardOrder;
                return order.IndexOf(mismatchPair.First) - order.IndexOf(mismatchPair.Second);
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

        public string CardsString { get; }

        public EHandType HandType { get; }

        public Hand(string cardsString, int bid, bool useJokers = false)
        {
            CardsString = cardsString;
            Cards = cardsString.ToCharArray();
            Bid = bid;

            Dictionary<char, int> cardCounts = Cards.GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());
            
            if (useJokers)
            {
                TransformJokers(cardCounts);
            }

            List<int> countsOrdered = cardCounts
                .Values
                .OrderByDescending(t => t)
                .ToList();
            
            HandType = countsOrdered switch
            {
                [1, ..] => EHandType.High,
                [2, 1, ..] => EHandType.OneP,
                [2, 2, ..] => EHandType.TwoP,
                [3, 1, ..] => EHandType.ThreeKind,
                [3, 2, ..] => EHandType.FullHouse,
                [4, ..] => EHandType.FourKind,
                [5, ..] => EHandType.FiveKind,
                _ => throw new ArgumentOutOfRangeException(string.Join(",", countsOrdered))
            };
        }

        public char[] Cards { get; init; }
        public int Bid { get; init; }

        private void TransformJokers(Dictionary<char, int> cardCounts)
        {
            if (!cardCounts.TryGetValue('J', out int jokerCount))
            {
                return;
            }

            if (jokerCount == 5)
            {
                return;
            }
            
            char jokerTransformsTo = cardCounts.Where(kvp => kvp.Key != 'J').MaxBy(kvp => kvp.Value).Key;
            cardCounts[jokerTransformsTo] += jokerCount;
            cardCounts.Remove('J');
        }
    }

    [ExpectedResult(248217452, Sample = 6440)]
    public object SolvePartOne(string input)
    {
        Hand.Comparer comparer = new(false);

        return InputHelpers.AsLines(input)
            .Select(l =>
            {
                string[] split = l.Split(' ');
                return new Hand(split[0], int.Parse(split[1]));
            })
            .OrderBy(h => h, comparer)
            .Select((hand, idx) => (hand, idx))
            .Aggregate(0, (total, tuple) => total + tuple.hand.Bid * (tuple.idx + 1));
    }

    [ExpectedResult(245576185, Sample = 5905)]
    public object SolvePartTwo(string input)
    {
        Hand.Comparer comparer = new(true);

        var list = InputHelpers.AsLines(input)
            .Select(l =>
            {
                string[] split = l.Split(' ');
                return new Hand(split[0], int.Parse(split[1]), useJokers: true);
            })
            .OrderBy(h => h, comparer)
            .ToList();

        return list.Select((hand, idx) => (hand, idx))
            .Aggregate(0, (total, tuple) => total + tuple.hand.Bid * (tuple.idx + 1));
    }
}