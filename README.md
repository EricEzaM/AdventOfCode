# Advent of Code C#
Advent of Code in C# - puzzle solutions & framework for easy creation of solution code.

## Usage
* Ensure your working directory is in the directory where `AdventOfCode.csproj` is locationed (currently `src/AdventOfCode/`) before running `dotnet run` commands.
* Use `dotnet user-secrets` or an environment variable to store your session id from `https://adventofcode.com/` once you have logged in. This allows downloading your input data automatically. 
  * `dotnet user-secrets set "AOC_SESSION" "<session>"`, or
  * Set environment variable `AOC_SESSION`

`dotnet run create <year> <day>`

Create the folder structure & solution `.cs` file for the given year and day. Also downloads the input file to `input.txt` if a valid session id is provided. The framework expects a `input.txt` file in the folder for each day.

`dotnet run results <year> <day>`

Runs the results for the given year and day.

More to come...