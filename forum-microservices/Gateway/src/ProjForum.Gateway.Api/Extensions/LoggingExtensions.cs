using Serilog;

namespace ProjForum.Gateway.Api.Extensions;

public static class LoggingExtensions
{
    public static IHostBuilder AddSerilogLogging(this IHostBuilder host, IConfiguration config)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });

        return host;
    }
}