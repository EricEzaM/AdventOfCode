# Advent of Code C#
Advent of Code in C# - puzzle solutions & framework for easy creation of solution code.

The goal of (most) of my solutions is to have them be as generic as possible - i.e. trying to avoid magic numbers that only apply to my dataset.

## Usage
* For puzzle folder creation, use `dotnet user-secrets` or an environment variable to store your session id from `https://adventofcode.com/` once you have logged in. This allows downloading your input data automatically. 
  * `dotnet user-secrets set "AOC_SESSION" "<session>"`, or
  * Set environment variable `AOC_SESSION`

Use with dotnet run:
`dotnet run <command> <args> <options>`
* Ensure your working directory is in the directory where `AdventOfCode.csproj` is locationed (currently `src/AdventOfCode/`) before running `dotnet run` commands.

#### Create
```
Description:
  Creates a solution template for a given year and day. Uses today if not year/day provided. Downloads input file to
  input.txt if a session cookie is provided via user secrets or environment variable AOC_SESSION.

Usage:
  AdventOfCode create [<year> [<day>]] [options]

Arguments:
  <year>  The year, or omit if today's year.
  <day>   The day, or omit if today's date.
```
#### Results
```
Description:
  Lists results, optionally for a specific year/day.

Usage:
  AdventOfCode results [<year> [<day>]] [options]

Arguments:
  <year>  The year filter.
  <day>   The day filter.

Options:
  --today          Output results for today's date.
  --sample         Run on sample file, if it exists.
  -md, --markdown  Display results table in markdown.
```

#### Benchmark
Uses BenchmarkDotnet - benchmark classes are auto generated through a source generator.

```
Description:
  Run a benchmark on a puzzle.

Usage:
  AdventOfCode bench <year> <day> [options]

Arguments:
  <year>  The year filter.
  <day>   The day filter.

Options:
  -q, --quick     Use quick benchmark configuration.
```