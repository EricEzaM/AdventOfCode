using System.CommandLine;
using System.Reflection;
using AdventOfCode.Lib.Cli;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Lib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCli(this IServiceCollection services)
    {
        services.AddCliRequiredServices();
        
        var commandType = typeof(Command);
        var commands = Assembly.GetExecutingAssembly()
                               .GetExportedTypes()
                               .Where(x => commandType.IsAssignableFrom(x));

        foreach (var command in commands)
        {
            services.AddSingleton(commandType, command);
        }

        return services;
    }

    private static IServiceCollection AddCliRequiredServices(this IServiceCollection services)
    {
        services.AddHttpClient<CreateCommand>()
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler()
                {
                    UseCookies = false
                });

        return services;
    }
}