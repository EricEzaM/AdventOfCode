using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using AdventOfCode.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var sp = BuildServiceProvider();
var p = BuildParser(sp);

return await p.InvokeAsync(args);

Parser BuildParser(IServiceProvider serviceProvider)
{
    var cliBuilder = new CommandLineBuilder(new RootCommand());

    foreach (var command in serviceProvider.GetServices<Command>())
    {
        cliBuilder.Command.AddCommand(command);
    }

    return cliBuilder.UseDefaults().Build();
}

ServiceProvider BuildServiceProvider()
{
    var services = new ServiceCollection();
    
    var config = new ConfigurationBuilder()
                 .AddUserSecrets(Assembly.GetExecutingAssembly())
                 .Build();

    services.AddSingleton<IConfiguration>(config);
    services.AddCli();

    return services.BuildServiceProvider();
}