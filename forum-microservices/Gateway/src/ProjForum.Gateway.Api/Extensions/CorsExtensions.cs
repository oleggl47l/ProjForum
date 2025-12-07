using Microsoft.Extensions.Options;
using ProjForum.Gateway.Api.Options;

namespace ProjForum.Gateway.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors((options) =>
        {
            options.AddDefaultPolicy((policy) =>
            {
                policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(services.BuildServiceProvider().GetRequiredService<IOptions<CorsPoliticsOptions>>()
                        .Value.Origin);
            });
        });

        return services;
    }
}