using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D05;

public class Y2023D05 : ISolution
{
    /// <summary>
    /// A single item from a map, e.g. 45 77 23
    /// </summary>
    private record MapItem(long DestinationStart, long SourceStart, long Count)
    {
        public long SourceEnd => SourceStart + Count - 1;

        public bool CanMap(long value) => value >= SourceStart && value < SourceStart + Count;

        public long GetMappedValue(long value)
        {
            if (!CanMap(value))
            {
                return value;
            }

            long delta = value - SourceStart;
            return DestinationStart + delta;
        }
    }

    /// <summary>
    /// A mapping, e.g. light-to-temperature
    /// </summary>
    private record Map(List<MapItem> MapItems)
    {
        public long GetMappedValue(long value)
        {
            return MapItems
                       .FirstOrDefault(i => i.CanMap(value))
                       ?.GetMappedValue(value)
                   ?? value;
        }
    }

    [ExpectedResult(1181555926, Sample = 35)]
    public object SolvePartOne(string input)
    {
        string[] lines = InputHelpers.AsLines(input);

        ParseMapsAndSeeds(lines, out List<long> seeds, out List<Map> maps);

        return seeds
            .Select(seed => maps.Aggregate(seed, (currentValue, map) => map.GetMappedValue(currentValue)))
            .Min();
    }

    /// <summary>
    /// The non-brute force solution to part 2 uses the fact that you can apply the mapping transforms to a range by just applying it to the first and last value of that range.
    /// This works because we are looking for the minimum, so by the end of the transformations we can just take the minimum value of the range.
    /// Note that Ranges may be split by mappings, e.g.
    /// |----range----|
    ///           |----map----|
    /// In which case that mapping cannot be applied to the complete range. In this case we need to split the range at the point where the mapping starts.
    /// |----range 1----||----range 2----|
    ///                  |-----------map---------|
    /// Now the mapping can just be applied to range 2, where it should be. Now we need to track range 1 and range 2 as we make mapping transforms from now on. 
    /// </summary>
    [ExpectedResult(37806486, Sample = 46)]
    public object SolvePartTwo(string input)
    {
        string[] lines = InputHelpers.AsLines(input);
        ParseMapsAndSeeds(lines, out List<long> seeds, out List<Map> maps);

        List<List<SeedRange>> seedRangeGroups = seeds
            .Chunk(2)
            .Select(sr => new SeedRange(sr[0], sr[0] + sr[1] - 1))
            .OrderBy(sr => sr.Start)
            .Select(sr => new List<SeedRange> { sr }) // A single SeedRange may be split into additional seed ranges later, so make this a list.
            .ToList();

        foreach (Map map in maps)
        {
            foreach (List<SeedRange> seedRanges in seedRangeGroups)
            {
                // First, process which seed ranges must be split into additional seed ranges because they intersect with a map
                foreach (MapItem mapItem in map.MapItems)
                {
                    var splitRanges = seedRanges
                        .SelectMany(sr => SplitRangeByMapItem(sr, mapItem))
                        .ToList();

                    seedRanges.Clear();
                    seedRanges.AddRange(splitRanges);
                }

                // Then, make the updates to the ranges based on the mappings.
                foreach (SeedRange range in seedRanges)
                {
                    foreach (MapItem mapItem in map.MapItems)
                    {
                        if (!mapItem.CanMap(range.Start))
                        {
                            continue;
                        }

                        range.Start = mapItem.GetMappedValue(range.Start);
                        range.End = mapItem.GetMappedValue(range.End);
                        break;
                    }
                }
            }
        }

        return seedRangeGroups.SelectMany(sr => sr).Min(sr => sr.Start);
    }

    /// <summary>
    /// Specifically used for Part 2, represents a range of seed values.
    /// </summary>
    private class SeedRange
    {
        public SeedRange(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long Start { get; set; }
        public long End { get; set; }
    }

    private IEnumerable<SeedRange> SplitRangeByMapItem(SeedRange range, MapItem mapItem)
    {
        // |----range----|
        //           |----map----|
        if (range.Start < mapItem.SourceStart && range.End > mapItem.SourceStart && range.End < mapItem.SourceEnd)
        {
            yield return new SeedRange(range.Start, mapItem.SourceStart - 1);
            yield return new SeedRange(mapItem.SourceStart, range.End);
            yield break;
        }

        //         |----range----|
        // |----map----|
        if (range.Start > mapItem.SourceStart && range.Start < mapItem.SourceEnd && range.End > mapItem.SourceEnd)
        {
            yield return new SeedRange(range.Start, mapItem.SourceEnd);
            yield return new SeedRange(mapItem.SourceEnd + 1, range.End);
            yield break;
        }

        // |-------range-------|
        //     |----map----|
        if (range.Start < mapItem.SourceStart && range.End > mapItem.SourceEnd)
        {
            yield return new SeedRange(range.Start, mapItem.SourceStart - 1);
            yield return new SeedRange(mapItem.SourceStart, mapItem.SourceEnd);
            yield return new SeedRange(mapItem.SourceEnd + 1, range.End);
            yield break;
        }

        yield return range;
    }
    
    private void ParseMapsAndSeeds(string[] lines, out List<long> seeds, out List<Map> maps)
    {
        seeds = new List<long>();
        maps = new List<Map>();

        Map? currentMap = null;
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.EndsWith("map:"))
            {
                currentMap = new Map(new List<MapItem>());
                maps.Add(currentMap);
                continue;
            }

            if (line.StartsWith("seeds:"))
            {
                seeds = line.Split(' ').Skip(1).Select(long.Parse).ToList();
                continue;
            }

            if (currentMap is null)
            {
                throw new ApplicationException("Current map is null - cannot add map items to null map. Something went wrong while parsing.");
            }

            // numbers
            var mapRangeNumbers = line.Split(' ').Select(long.Parse).ToArray();
            currentMap.MapItems.Add(new MapItem(mapRangeNumbers[0], mapRangeNumbers[1], mapRangeNumbers[2]));
        }
    }
}