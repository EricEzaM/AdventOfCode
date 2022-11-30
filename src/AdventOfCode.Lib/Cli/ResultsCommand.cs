using System.CommandLine;
using System.Reflection;

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
        var solutions = Assembly
                        .GetEntryAssembly()!
                        .GetTypes()
                        .Where(t => t.GetInterface(nameof(ISolution)) is not null)
                        .OrderBy(t => t.FullName);

        if (today)
        {
            filterYear = DateTime.Now.Year;
            filterDay = DateTime.Now.Day;
        }
        
        foreach (var solutionType in solutions)
        {
            if (Activator.CreateInstance(solutionType) is not ISolution solution)
            {
                continue;
            }
            
            string[] namespaceComponents = solutionType.Namespace!.Split(".");

            string yearText = namespaceComponents.Skip(1).Take(1).Single();
            string dayText = namespaceComponents.Skip(2).Take(1).Single();
                
            int year = int.Parse(yearText.TrimStart('Y'));
            int day = int.Parse(dayText.TrimStart('D'));

            if ((filterYear != null && year != filterYear) || (filterDay != null && day != filterDay))
            {
                continue;
            }
                
            Console.WriteLine($"***** SOLUTION {yearText} {dayText} *****");
            string inputFile = Path.Combine(yearText, dayText, useSample ? "sample.txt" : "input.txt");
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"Input file '{inputFile}' cannot be found!", inputFile);
            }
            
            string input = File.ReadAllText(inputFile);

            Console.WriteLine($"Part One = {solution.SolvePartOne(input)}");
            Console.WriteLine($"Part Two = {solution.SolvePartTwo(input)}");
        }
    }
}