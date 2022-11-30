using System.CommandLine;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace AdventOfCode.Lib.Cli;

public class CreateCommand : Command
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public CreateCommand(IConfiguration config, HttpClient httpClient) : base("create", "Creates a solution template for a given year and day.")
    {
        _config = config;
        _httpClient = httpClient;
        var yearArg = new Argument<int?>("year");
        var dayArg = new Argument<int?>("day");
        
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

        string dir = Path.Combine($"Y{year}", $"D{day:00}");
        Directory.CreateDirectory(dir);

        string solutionFilePath = Path.Combine(dir, $"Y{year.Value}D{day.Value:00}.cs");

        if (File.Exists(solutionFilePath))
        {
            throw new ApplicationException("It appears the solution scaffolding has already been done! The .cs file exists!");
        }
        
        string solutionFileContent = GenerateSolutionFile(year.Value, day.Value);
        await File.WriteAllTextAsync(solutionFilePath, solutionFileContent);
        Console.WriteLine($"Solution file created at {solutionFilePath}");
        
        var req = new HttpRequestMessage(HttpMethod.Get, $"https://adventofcode.com/{year}/day/{day}/input");
        req.Headers.Add("Cookie", $"session={GetSession()}");
        var res = await _httpClient.SendAsync(req);

        if (res.IsSuccessStatusCode)
        {
            string contentFilePath = Path.Combine(dir, "input.txt");
            var content = await res.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(contentFilePath, content.TrimEnd('\n'));
            
            Console.WriteLine($"Input file created at {contentFilePath}");
        }
        else
        {
            Console.WriteLine($"Request for input failed ({req.RequestUri})");
        }
        
        return;
    }

    private string GetSession() =>
        Environment.GetEnvironmentVariable("AOC_SESSION")
        ?? _config["AOC_SESSION"]
        ?? throw new ApplicationException("AOC_SESSION is not set in environment or configuration (via secrets).");

    private string GenerateSolutionFile(int year, int day)
    {
        var file = @$"using AdventOfCode.Lib;
                    |
                    |namespace AdventOfCode.Y{year}.D{day:00};
                    |
                    |public class Y{year}D{day:00} : ISolution
                    |{{
                    |    public object SolvePartOne(string input)
                    |    {{
                    |        return string.Empty;
                    |    }}
                    |
                    |    public object SolvePartTwo(string input)
                    |    {{
                    |        return string.Empty;
                    |    }}
                    |}}";

        var replaced = Regex.Replace(file, @"\s+\|", "\n");
        return replaced;
    }
}