using System.CommandLine;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace AdventOfCode.Lib.Cli;

public class CreateCommand : Command
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public CreateCommand(IConfiguration config, HttpClient httpClient) : base("create", "Creates a solution template for a given year and day. Uses today if not year/day provided.")
    {
        _config = config;
        _httpClient = httpClient;
        var yearArg = new Argument<int?>("year", "The year, or omit if today's year.");
        var dayArg = new Argument<int?>("day", "The day, or omit if today's date.");
        
        Add(yearArg);
        Add(dayArg);
        
        this.SetHandler(HandleCommand, yearArg, dayArg);
    }

    private async Task HandleCommand(int? year, int? day)
    {
        if (year == null && day == null)
        {
            year = DateTime.Now.Year;
            day = DateTime.Now.Day;
        }

        if (day == null || year == null)
        {
            throw new ApplicationException("Year or day is not provided");
        }

        AnsiConsole.WriteLine($"Creating solution structure for Y={year} D={day}");
        
        // Create .cs puzzle solution file. If it already exists, create another with 'A{X}' appended
        // for alternative solutions.
        string dir = Path.Combine($"Y{year}", $"D{day:00}");
        Directory.CreateDirectory(dir);

        int fileCount = 0;
        string csPath = Path.Combine(dir, $"Y{year.Value}D{day.Value:00}.cs");
        while (File.Exists(csPath))
        {
            string replaceMe = fileCount == 0 ? "." : $"A{fileCount}.";
            fileCount++;
            csPath = csPath.Replace(replaceMe, $"A{fileCount}.");
        }
        
        string solutionFileContent = GenerateSolutionFile(year.Value, day.Value, fileCount);
        await File.WriteAllTextAsync(csPath, solutionFileContent);
        Console.WriteLine($"Solution file #{fileCount} created at {csPath}");

        // Create the input.txt file by downloading it from the advent of code site.
        // Must have the SESSION cookie value configured as secret or env var.
        string inputFilePath = Path.Combine(dir, "input.txt");
        if (File.Exists(inputFilePath))
        {
            Console.WriteLine($"Input file already exists at {inputFilePath}");
            return;
        }
        
        var req = new HttpRequestMessage(HttpMethod.Get, $"https://adventofcode.com/{year}/day/{day}/input");
        req.Headers.Add("Cookie", $"session={GetSession()}");
        var res = await _httpClient.SendAsync(req);

        if (res.IsSuccessStatusCode)
        {
            var content = await res.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(inputFilePath, content.TrimEnd('\n'));
            
            Console.WriteLine($"Input file created at {inputFilePath}");
        }
        else
        {
            Console.WriteLine($"Request for input failed ({req.RequestUri})");
        }
    }

    private string GetSession() =>
        Environment.GetEnvironmentVariable("AOC_SESSION")
        ?? _config["AOC_SESSION"]
        ?? throw new ApplicationException("AOC_SESSION is not set in environment or configuration (via secrets).");

    private string GenerateSolutionFile(int year, int day, int iteration)
    {
        string iterationString = iteration == 0 ? "" : $"A{iteration}";

        var comment = iteration == 0 ? "" : 
            $@"
            |/// <summary>
            |/// Alternative solution #{iteration} 
            |/// </summary>";
        
        var file = @$"using AdventOfCode.Lib;
                    |using AdventOfCode.Lib.Attributes;
                    |
                    |namespace AdventOfCode.Y{year}.D{day:00};
                    |{comment}
                    |public class Y{year}D{day:00}{iterationString} : ISolution
                    |{{
                    |    [ExpectedResult("""", Sample = """")]
                    |    public object SolvePartOne(string input)
                    |    {{
                    |        return string.Empty;
                    |    }}
                    |
                    |    [ExpectedResult("""", Sample = """")]
                    |    public object SolvePartTwo(string input)
                    |    {{
                    |        return string.Empty;
                    |    }}
                    |}}";

        var replaced = Regex.Replace(file, @"\s+\|", "\n");
        return replaced;
    }
}