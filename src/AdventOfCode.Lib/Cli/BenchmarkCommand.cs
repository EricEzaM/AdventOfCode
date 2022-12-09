using System.CommandLine;
using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace AdventOfCode.Lib.Cli;

public class BenchmarkCommand : Command
{
    public BenchmarkCommand() : base("bench", "Run a benchmark on a puzzle.")
    {
        var yearArg = new Argument<int>("year", "The year filter.");
        var dayArg = new Argument<int>("day", "The day filter.");
        
        var quickOpt = new Option<bool>(new []{"--quick", "-q"}, "Use quick benchmark configuration.");
        
        Add(yearArg);
        Add(dayArg);
        Add(quickOpt);
        
        this.SetHandler(HandleCommand, yearArg, dayArg, quickOpt);
    }

    private void HandleCommand(int year, int day, bool quick)
    {
        Console.WriteLine($"Running benchmark for Y={year} D={day}");

        var config = ManualConfig
                     .CreateMinimumViable()
                     .AddJob(quick ? Job.ShortRun : Job.Default)
                     .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(false)))
                     .AddFilter(new SimpleFilter(c => c.Descriptor.Type.Name.Contains($"Y{year}D{day:00}")));

        BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly())
                         .RunAllJoined(config);
    }
}