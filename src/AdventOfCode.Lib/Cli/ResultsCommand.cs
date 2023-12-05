using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using AdventOfCode.Lib.Attributes;
using Spectre.Console;

namespace AdventOfCode.Lib.Cli;

public class ResultsCommand : Command
{
    public ResultsCommand() : base("results", "Lists results, optionally for a specific year/day.")
    {
        var yearOpt = new Argument<int?>("year", "The year filter.");
        var dayOpt = new Argument<int?>("day", "The day filter.");
        var todayOpt = new Option<bool>("--today", "Output results for today's date.");
        var sampleOpt = new Option<bool>("--sample", "Run on sample file, if it exists.");
        var mdOpt = new Option<bool>(new[] { "--markdown", "-md" }, "Display results table in markdown.");

        Add(yearOpt);
        Add(dayOpt);
        Add(todayOpt);
        Add(sampleOpt);
        Add(mdOpt);

        this.SetHandler(HandleCommand, yearOpt, dayOpt, todayOpt, sampleOpt, mdOpt);
    }

    private void HandleCommand(int? filterYear, int? filterDay, bool today, bool useSample, bool useMarkdown)
    {
        var solutionTypes = Assembly
            .GetEntryAssembly()!
            .GetTypes()
            .Where(t => t.GetInterface(nameof(ISolution)) is not null)
            .OrderBy(t => t.FullName);

        if (today)
        {
            filterYear = DateTime.Now.Year;
            filterDay = DateTime.Now.Day;
        }

        AnsiConsole.WriteLine($"Solutions for Y={filterYear} D={filterDay}");

        var errs = new List<string>();
        var table = new Table();

        if (useMarkdown)
        {
            table.MarkdownBorder();
        }

        table.AddColumns("Year", "Day", "Alt", "P1", "P1 time", "P1 mem delta", "P2", "P2 time", "P2 mem delta");
        foreach (var solutionType in solutionTypes)
        {
            if (Activator.CreateInstance(solutionType) is not ISolution solution)
            {
                continue;
            }

            if (!DoesClassPassFilter(solutionType, filterYear, filterDay, out string yearText, out string dayText))
            {
                continue;
            }

            int altNum = GetAlternativeNumber(solutionType);
            string inputFilePart1 = Path.Combine(yearText, dayText, useSample ? "sample.txt" : "input.txt");
            if (!File.Exists(inputFilePart1))
            {
                table.AddRow(yearText, dayText, altNum.ToString(), "???", "???", "???", "???");
                errs.Add($"Failed to find input file for {yearText} {dayText}: {inputFilePart1}");
                continue;
            }

            string input = File.ReadAllText(inputFilePart1);

            // Handle having a 2nd file for input specifically for part 2
            string inputPart2 = input;
            string inputFilePart2 = Path.Combine(yearText, dayText, useSample ? "sample-pt2.txt" : "input.txt");
            if (useSample && File.Exists(inputFilePart2))
            {
                inputPart2 = File.ReadAllText(inputFilePart2);
            }

            var (p1, p1Time, p1Mem) = GetSolutionResult(() => solution.SolvePartOne(input));
            var (p2, p2Time, p2Mem) = GetSolutionResult(() => solution.SolvePartTwo(inputPart2));

            var p1Expected = GetExpected(solutionType, nameof(ISolution.SolvePartOne), useSample);
            var p2Expected = GetExpected(solutionType, nameof(ISolution.SolvePartTwo), useSample);

            table.AddRow(
                new Text(yearText),
                new Text(dayText),
                new Text(altNum.ToString()),
                new Text(p1.ToString() ?? string.Empty, GetStyle(p1Expected, p1)),
                new Text(p1Time),
                new Text($"{p1Mem / 1024d:G6} kB"),
                new Text(p2.ToString() ?? string.Empty, GetStyle(p2Expected, p2)),
                new Text(p2Time),
                new Text($"{p2Mem / 1024d:G6} kB"));
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("* Memory usage estimate is very close to BenchmarkDotnet results.");
        AnsiConsole.WriteLine("* Timing estimates will be a lot higher than BenchmarkDotnet results.");
        foreach (string err in errs)
        {
            AnsiConsole.WriteLine(err);
        }
    }

    private (object result, string time, long mem) GetSolutionResult(Func<object> action)
    {
        var sw = Stopwatch.StartNew();
        long memBefore = GC.GetTotalMemory(true);
        object result = action();
        long memAfter = GC.GetTotalMemory(false);
        string time = $"{sw.Elapsed.TotalMilliseconds} ms";

        return (result, time, memAfter - memBefore);
    }

    private int GetAlternativeNumber(Type type)
    {
        var match = Regex.Match(type.Name, @"A(\d+)$");
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }

        return 0;
    }

    private object? GetExpected(Type type, string methodName, bool sample)
    {
        var attr = type.GetMethod(methodName)!.GetCustomAttribute<ExpectedResultAttribute>();
        return sample ? attr?.Sample : attr?.Expected;
    }

    private Style GetStyle(object? expected, object actual)
    {
        if (expected is null)
        {
            return Style.Plain;
        }

        if (expected.Equals(actual))
        {
            return new Style(Color.Green);
        }

        if (expected.GetType() == actual.GetType())
        {
            return new Style(Color.Red);
        }

        try
        {
            object newExpected = Convert.ChangeType(expected, actual.GetType());
            return GetStyle(newExpected, actual);
        }
        catch (Exception)
        {
            // Convert failed
            return new Style(Color.Red);
        }
    }

    private bool DoesClassPassFilter(Type type, int? filterYear, int? filterDay, out string yearText,
        out string dayText)
    {
        string[] namespaceComponents = type.Namespace!.Split(".");

        yearText = namespaceComponents.Skip(1).Take(1).Single();
        dayText = namespaceComponents.Skip(2).Take(1).Single();

        int year = int.Parse(yearText.TrimStart('Y'));
        int day = int.Parse(dayText.TrimStart('D'));

        if (filterYear is null)
        {
            return true;
        }

        if (filterDay is null)
        {
            return filterYear == year;
        }

        return filterYear == year && filterDay == day;
    }
}