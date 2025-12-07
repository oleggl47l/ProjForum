using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjForum.Gateway.Api.Options;

namespace ProjForum.Gateway.Api.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", (options) =>
            {
                var jwt = services.BuildServiceProvider()
                    .GetRequiredService<IOptions<JwtAuthenticationOptions>>().Value;

                options.Authority = jwt.Authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return services;
    }
}