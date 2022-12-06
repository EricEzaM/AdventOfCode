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
        var sampleOpt = new Option<bool>("--sample", "Run on sample file, if it exists?");

        Add(yearOpt);
        Add(dayOpt);
        Add(todayOpt);
        Add(sampleOpt);

        this.SetHandler(HandleCommand, yearOpt, dayOpt, todayOpt, sampleOpt);
    }

    private void HandleCommand(int? filterYear, int? filterDay, bool today, bool useSample)
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
        table.AddColumns("Year", "Day", "Alt", "P1", "P1 time", "P1 mem kb delta", "P2", "P2 time", "P2 mem kb delta");
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
            string inputFile = Path.Combine(yearText, dayText, useSample ? "sample.txt" : "input.txt");
            if (!File.Exists(inputFile))
            {
                table.AddRow(yearText, dayText, altNum.ToString(), "???", "???", "???", "???");
                errs.Add($"Failed to find input file for {yearText} {dayText}: {inputFile}");
                continue;
            }

            string input = File.ReadAllText(inputFile);

            var (p1, p1Time, p1Mem) = GetSolutionResult(() => solution.SolvePartOne(input));
            var (p2, p2Time, p2Mem) = GetSolutionResult(() => solution.SolvePartTwo(input));

            var p1Expected = GetExpected(solutionType, nameof(ISolution.SolvePartOne), useSample);
            var p2Expected = GetExpected(solutionType, nameof(ISolution.SolvePartTwo), useSample);

            table.AddRow(
                new Text(yearText),
                new Text(dayText),
                new Text(altNum.ToString()),
                new Text(p1.ToString() ?? string.Empty, GetStyle(p1Expected, p1)),
                new Text(p1Time),
                new Text($"{p1Mem / 1024d:G6}"),
                new Text(p2.ToString() ?? string.Empty, GetStyle(p2Expected, p2)),
                new Text(p2Time),
                new Text($"{p2Mem / 1024d:G6}"));
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
        long memBefore = GC.GetTotalMemory(false);
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

    private Style GetStyle(object? expected, object actual) =>
        expected is null
            ? Style.Plain
            : new Style(expected.Equals(actual) ? Color.Green : Color.Red);

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