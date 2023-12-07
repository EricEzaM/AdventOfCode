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

        public Hand(string cardsString, int bid, bool considerJokers = false)
        {
            CardsString = cardsString;
            Cards = cardsString.ToCharArray();
            Bid = bid;

            List<(char card, int count)> charCounts = cardsString.GroupBy(c => c)
                .Select(g => (card: g.Key, count: g.Count()))
                .OrderByDescending(t => t.count)
                .ToList();

            char primaryCard = charCounts[0].card;
            HandType = charCounts.Select(t => t.count).ToList() switch
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

            if (considerJokers)
            {
                EHandType originalType = HandType;
                int jCount = Cards.Count(c => c == 'J');
                for (int i = 0; i < jCount; i++)
                {
                    HandType = ApplyJoker(originalType, HandType, primaryCard);
                }
            }
        }

        public char[] Cards { get; init; }
        public int Bid { get; init; }

        private EHandType ApplyJoker(EHandType originalType, EHandType currentType, char primaryCard)
        {
            if (primaryCard == 'J' && originalType == EHandType.OneP)
            {
                return EHandType.ThreeKind;
            }
            
            return currentType switch
            {
                EHandType.High => EHandType.OneP,
                EHandType.OneP => EHandType.ThreeKind,
                EHandType.TwoP => EHandType.FullHouse,
                EHandType.ThreeKind => EHandType.FourKind,
                EHandType.FullHouse => EHandType.FourKind,
                EHandType.FourKind => EHandType.FiveKind,
                EHandType.FiveKind => EHandType.FiveKind,
                _ => throw new ArgumentOutOfRangeException(nameof(currentType), currentType, null)
            };
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
                return new Hand(split[0], int.Parse(split[1]), considerJokers: true);
            })
            .OrderBy(h => h, comparer)
            .ToList();

        foreach (var hand in list)
        {
            Console.WriteLine($"{hand.CardsString} = {hand.HandType}");
        }
        
        return list.Select((hand, idx) => (hand, idx))
            .Aggregate(0, (total, tuple) => total + tuple.hand.Bid * (tuple.idx + 1));
    }
}