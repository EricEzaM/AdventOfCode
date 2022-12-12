using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D11;

public class Y2022D11 : ISolution
{
    private static long SuperMod = long.MaxValue;
    
    /// <summary>
    /// Monkey data structure
    /// </summary>
    /// <param name="Operation">The operation to perform when calculating new worry - *,/,-,+</param>
    /// <param name="Operand">The operand of the operation. If null, uses the current worry. e.g. Operation = *, Operand = null, result = worry * worry.</param>
    /// <param name="Items">The items the monkey has.</param>
    /// <param name="TestDivisor">The divisor for the monkeys test</param>
    /// <param name="TargetTrue">The target monkey id if the test result is true.</param>
    /// <param name="TargetFalse">The target monkey id if the test result is false.</param>
    /// <param name="WorryRelief">The amount worry is divided by after the monkey inspects the item.</param>
    private record Monkey(char Operation, long? Operand, Queue<long> Items, long TestDivisor,
                          int TargetTrue,
                          int TargetFalse, long WorryRelief)
    {
        public long InspectedCount { get; private set; }

        public IEnumerable<(int targetMonkeyIdx, long worry)> InspectItems()
        {
            while (Items.TryDequeue(out long worry))
            {
                checked
                {
                    InspectedCount++;
                    long newWorry = Operation switch
                    {
                        '*' => (worry * (Operand ?? worry)) / WorryRelief,
                        '+' => (worry + (Operand ?? worry)) / WorryRelief,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                    // newWorry % SuperMod (for part 2) to ensure it stays within a lower range without
                    // impacting calculations related to the divisor. SuperMod is the LCM of all the TestDivisors for
                    // all monkeys.
                    yield return (newWorry % TestDivisor == 0 ? TargetTrue : TargetFalse, newWorry % SuperMod);
                }
            }
        }
    }

    [ExpectedResult(50830L, Sample = 10605L)]
    public object SolvePartOne(string input)
    {
        var monkeys = ParseMonkeys(input);

        for (long i = 0; i < 20; i++)
        {
            foreach ((int target, long worry) in monkeys.SelectMany(m => m.InspectItems()))
            {
                monkeys[target].Items.Enqueue(worry);
            }
        }

        return monkeys.OrderByDescending(m => m.InspectedCount)
                      .Take(2)
                      .Aggregate(1L, (agg, monkey) => agg * monkey.InspectedCount);
    }

    // Cheated for this one. Did not know the modulo stuff off the top of my head.
    [ExpectedResult(2713310158L, Sample = 2713310158L)]
    public object SolvePartTwo(string input)
    {
        var monkeys = ParseMonkeys(input, 1);

        SuperMod = monkeys.Aggregate(1L, (agg, monkey) => agg * monkey.TestDivisor);
        Console.WriteLine($"SuperMod is {SuperMod}");
        
        for (int i = 0; i < 10000; i++)
        {
            foreach ((int target, long worry) in monkeys.SelectMany(m => m.InspectItems()))
            {
                monkeys[target].Items.Enqueue(worry);
            }
        }
        
        return monkeys.OrderByDescending(m => m.InspectedCount)
                      .Take(2)
                      .Aggregate(1L, (agg, m) => agg * m.InspectedCount);
    }

    private Monkey[] ParseMonkeys(string input, long worryRelief = 3) =>
        input.Split("\n\n")
             .Select(s => InputHelpers.AsLines(s).ToArray())
             .Select(mbLines =>
                         new Monkey(mbLines[2][23],
                                    long.TryParse(mbLines[2][25..], out long operand) ? operand : null,
                                    new Queue<long>(mbLines[1][18..].Split(',').Select(v => long.Parse(v.Trim()))),
                                    long.Parse(mbLines[3][21..]),
                                    int.Parse(mbLines[4][29..]),
                                    int.Parse(mbLines[5][30..]),
                                    worryRelief
                         ))
             .ToArray();
}