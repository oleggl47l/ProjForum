using Microsoft.OpenApi.Models;

namespace ProjForum.Gateway.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddDocumentation(this IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gateway API", Version = "v1" }); });

        services.AddSwaggerForOcelot(config);

        return services;
    }
}