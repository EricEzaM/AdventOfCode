using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D05;

public class Y2023D05 : ISolution
{
    private record MapItem(ulong DestinationStart, ulong SourceStart, ulong Count)
    {
        public bool CanMap(ulong value) => value >= SourceStart && value < SourceStart + Count;

        public ulong GetMappedValue(ulong value)
        {
            if (!CanMap(value))
            {
                return value;
            }

            ulong delta = value - SourceStart;
            return DestinationStart + delta;
        }
    }

    private record Map(string From, string To, List<MapItem> MapItems)
    {
        public ulong GetMappedValue(ulong value)
        {
            return MapItems
                       .FirstOrDefault(i => i.CanMap(value))
                       ?.GetMappedValue(value)
                   ?? value;
        }
    }

    [ExpectedResult("", Sample = 35)]
    public object SolvePartOne(string input)
    {
        string[] lines = InputHelpers.AsLines(input);

        ParseMapsAndSeeds(lines, out List<ulong> seeds, out List<Map> maps);

        return seeds
            .Select(seed => GetMappedValue(maps, "seed", "location", seed))
            .Min();
    }

    [ExpectedResult("", Sample = "")]
    public object SolvePartTwo(string input)
    {
        return string.Empty;
    }

    private ulong GetMappedValue(List<Map> maps, string from, string to, ulong value)
    {
        var mappingOrder = GetMappingOrder(maps, from, to);
        ulong currentValue = value;
        foreach (Map map in mappingOrder)
        {
            currentValue = map.GetMappedValue(currentValue);
        }

        return currentValue;
    }

    private List<Map> GetMappingOrder(List<Map> inputMaps, string from, string to)
    {
        string currentFrom = from;
        List<Map> mapOrder = new();
        while (true)
        {
            Map? map = inputMaps.FirstOrDefault(m => m.From == currentFrom);
            if (map is null)
            {
                break;
            }

            mapOrder.Add(map);
            currentFrom = map.To;
        }

        if (mapOrder.Last().To != to)
        {
            throw new ApplicationException($"Search failed for {from} -> {to}, map order is: {string.Join(", ", mapOrder.Select(m => $"{m.From} -> {m.To}"))}");
        }

        return mapOrder;
    }

    private void ParseMapsAndSeeds(string[] lines, out List<ulong> seeds, out List<Map> maps)
    {
        seeds = new List<ulong>();
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
                var mapFromTo = line.Split(' ')[0].Split('-');
                currentMap = new Map(mapFromTo[0], mapFromTo[2], new List<MapItem>());
                maps.Add(currentMap);
                continue;
            }
            
            if (line.StartsWith("seeds:"))
            {
                seeds = line.Split(' ').Skip(1).Select(ulong.Parse).ToList();
                continue;
            }
            
            if (currentMap is null)
            {
                throw new ApplicationException("Current map is null - cannot add map items to null map. Something went wrong while parsing.");
            }
            
            // numbers
            var mapRangeNumbers = line.Split(' ').Select(ulong.Parse).ToArray();
            currentMap.MapItems.Add(new MapItem(mapRangeNumbers[0], mapRangeNumbers[1], mapRangeNumbers[2]));
        }
    }
}