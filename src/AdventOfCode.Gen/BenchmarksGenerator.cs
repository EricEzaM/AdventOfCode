using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace AdventOfCode.Gen;

[Generator]
public class BenchmarksGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
// #if DEBUG
//         if (!Debugger.IsAttached)
//         {
//             Debugger.Launch();
//         }
// #endif 
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var solutionFiles = context.Compilation.SyntaxTrees
                                   .Where(s => Regex.IsMatch(s.FilePath, @"Y\d{4}D\d{2}(A\d+)?\.cs"))
                                   .Select(s => s.FilePath)
                                   .ToList();
        
        foreach (var file in solutionFiles)
        {
            var dir = Path.GetDirectoryName(file);
            string input = File.ReadAllText(Path.Combine(dir, "input.txt"), Encoding.UTF8);
            string className = Path.GetFileName(file).Replace(".cs", "");

            var match = Regex.Match(className, @"Y(\d{4})D(\d{2})");
            
            string source = $@"// auto-generated
using AdventOfCode.Y{match.Groups[1]}.D{match.Groups[2]};
using AdventOfCode.Lib;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Benchmarks;

public class {className}Benchmark
{{
private string _input = @""{input}"";
private ISolution _solution;

[GlobalSetup]
public void GlobalSetup()
{{
    _solution = new {className}();
}}

[Benchmark]
public void PartOne()
{{
    _solution.SolvePartOne(_input);
}}

[Benchmark]
public void PartTwo()
{{
    _solution.SolvePartTwo(_input);
}}
}}
";
            context.AddSource($"{className}Benchmark.g.cs", source);
            
        }
    }
}