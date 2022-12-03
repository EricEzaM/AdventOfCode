﻿using System.CommandLine;
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
        var yearOpt = new Argument<int?>("year");
        var dayOpt = new Argument<int?>("day");
        var todayOpt = new Option<bool>("--today");
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

        AnsiConsole.WriteLine($"Solutions for Y={filterYear ?? '*'} D={filterDay ?? '*'}");

        var errs = new List<string>();
        var table = new Table();
        table.AddColumns("Year", "Day", "Alt", "P1", "P1 time", "P2", "P2 time");
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
            
            var sw = Stopwatch.StartNew();
            var p1 = solution.SolvePartOne(input);
            var p1Time = $"{sw.Elapsed.TotalMilliseconds:F3} ms";
            
            sw.Restart();
            var p2 = solution.SolvePartTwo(input);
            var p2Time = $"{sw.Elapsed.TotalMilliseconds:F3} ms";
            sw.Stop();

            var p1Expected = GetExpected(solutionType, nameof(ISolution.SolvePartOne), useSample);
            var p2Expected = GetExpected(solutionType, nameof(ISolution.SolvePartTwo), useSample);

            var p1Text = new Text(p1.ToString() ?? string.Empty, GetStyle(p1Expected, p1));
            var p2Text = new Text(p2.ToString() ?? string.Empty, GetStyle(p2Expected, p2));

            table.AddRow(
                new Text(yearText),
                new Text(dayText),
                new Text(altNum.ToString()),
                p1Text,
                new Text(p1Time),
                p2Text,
                new Text(p2Time));
        }
        
        AnsiConsole.Write(table);
        foreach (string err in errs)
        {
            AnsiConsole.WriteLine(err);
        }
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

    private bool DoesClassPassFilter(Type type, int? filterYear, int? filterDay, out string yearText, out string dayText)
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