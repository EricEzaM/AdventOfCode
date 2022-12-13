using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D13;

public class Y2022D13 : ISolution
{
    [ExpectedResult(5185, Sample = 13)]
    public object SolvePartOne(string input)
    {
        return input.Split("\n\n")
                         .Select(pair =>
                         {
                             var split = pair.Split('\n');
                             return (new PacketPart(split[0]), new PacketPart(split[1]));
                         })
                         .Select((pair, i) => (p: pair, i))
                         .Aggregate(0, (tot, v) => tot + (v.p.Item1.CompareTo(v.p.Item2) != 1 ? v.i + 1 : 0));
    }

    [ExpectedResult(23751, Sample = 140)]
    public object SolvePartTwo(string input)
    {
        return input.Split("\n")
                         .Where(l => !string.IsNullOrEmpty(l))
                         .Append("[[2]]")
                         .Append("[[6]]")
                         .Select(l => new PacketPart(l))
                         .Order()
                         .Select((p, i) => (p, i))
                         .Where(v => v.p.ToString() is "[[2]]" or "[[6]]")
                         .Aggregate(1, (tot, v) =>  tot * (v.i + 1));
    }

    private class PacketPart : IComparable<PacketPart>
    {
        private int? Integer { get; }
        private List<PacketPart>? SubParts { get; set; }

        private PacketPart()
        {
            SubParts = new();
        }

        private PacketPart(params PacketPart[] subParts)
        {
            SubParts = new(subParts);
        }

        private PacketPart(int integer)
        {
            Integer = integer;
        }

        public PacketPart(string packetLine)
        {
            // Skip first [ and last ]
            var charArray = packetLine.ToCharArray()[1..^1];

            Stack<PacketPart> subArrayParts = new();
            SubParts = new();

            for (int i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];
                switch (c)
                {
                    case '[':
                        subArrayParts.Push(new PacketPart());
                        break;
                    case ']':
                        if (subArrayParts.Count > 1)
                        {
                            var popped = subArrayParts.Pop();
                            subArrayParts.Peek().AddPart(popped);
                        }
                        else
                        {
                            SubParts.Add(subArrayParts.Pop());
                        }

                        break;
                    case ',':
                        break;
                    default:
                        var intStr = new string(charArray.Skip(i).TakeWhile(char.IsDigit).ToArray());
                        var intPart = new PacketPart(int.Parse(intStr));
                        if (subArrayParts.Count > 0)
                        {
                            subArrayParts.Peek().AddPart(intPart);
                        }
                        else
                        {
                            SubParts.Add(intPart);
                        }

                        i += intStr.Length - 1;
                        break;
                }
            }
        }

        public override string ToString()
        {
            if (Integer.HasValue)
            {
                return Integer.ToString()!;
            }

            return $"[{string.Join(",", SubParts!.Select(p => p.ToString()))}]";
        }

        public int CompareTo(PacketPart? r)
        {
            if (r is null)
            {
                return -1;
            }

            // 2 ints
            if (Integer.HasValue && r.Integer.HasValue)
            {
                return Integer.Value.CompareTo(r.Integer.Value);
            }

            // 2 lists
            if (SubParts is not null && r.SubParts is not null)
            {
                foreach (var (lsp, rsp) in SubParts.Zip(r.SubParts))
                {
                    var comp = lsp.CompareTo(rsp);
                    if (comp != 0)
                    {
                        return comp;
                    }
                }

                if (SubParts.Count != r.SubParts.Count)
                {
                    return SubParts.Count < r.SubParts.Count ? -1 : 1;
                }

                return 0;
            }

            // Integer and list
            return Integer.HasValue
                ? new PacketPart(new PacketPart(Integer.Value)).CompareTo(r)
                : CompareTo(new PacketPart(new PacketPart(r.Integer.Value)));
        }

        private void AddPart(PacketPart packetPart)
        {
            SubParts ??= new List<PacketPart>();
            SubParts.Add(packetPart);
        }
    }
}