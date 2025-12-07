using Ocelot.DependencyInjection;

namespace ProjForum.Gateway.Api.Extensions;

public static class OcelotExtensions
{
    public static void AddGateway(this IServiceCollection services, IConfiguration config)
    {
        services.AddOcelot(config);
    }
}