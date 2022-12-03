using AdventOfCode.Lib;

namespace AdventOfCode.Y2022.D03;

public class Y2022D03 : ISolution
{
    class Rucksack
    {
        public Rucksack(string contents)
        {
            Contents = contents;
            C1 = Contents.Substring(0, contents.Length / 2);
            C2 = Contents.Substring(contents.Length / 2);
        }
        
        public string Contents { get; }
        public string C1 { get; }
        public string C2 { get; }

        public IEnumerable<char> GetShared()
        {
            return C1.Intersect(C2);
        }
    }

    private readonly List<char> _chars = Enumerable.Repeat('a', 26)
        .Select((c, idx) => (char)(c + idx))
        .Concat(Enumerable.Repeat('A', 26)
            .Select((c, idx) => (char)(c + idx)))
        .ToList();

    public object SolvePartOne(string input)
    {
        return GetRucksacks(input)
            .Select(r => r.GetShared())
            .SelectMany(shared => shared)
            .Select(c => _chars.IndexOf(c) + 1)
            .Sum();
    }

    public object SolvePartTwo(string input)
    {
        return GetRucksacks(input)
            .Chunk(3)
            .Select(GetCommonItem)
            .Sum(c => _chars.IndexOf(c) + 1);
    }
    
    private char GetCommonItem(Rucksack[] rucksacks)
    {
        return rucksacks
            .Select(r => r.Contents)
            .Aggregate(string.Concat(_chars), (common, content) => string.Concat(common.Intersect(content)))
            .First();
    }

    private IEnumerable<Rucksack> GetRucksacks(string input)
    {
        return input.Split('\n')
            .Select(l => new Rucksack(l));
    }
}