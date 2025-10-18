using Microsoft.OpenApi.Models;
using ProjForum.Identity.Api.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjForum.Identity.Api.Extensions;

public static class SwaggerExtension
{
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration,
        Action<SwaggerGenOptions>? configure = null)
    {
        services.AddEndpointsApiExplorer();

        var swaggerOptions = configuration.GetSection("SwaggerDocOptions").Get<SwaggerDocOptions>()
                             ?? throw new InvalidOperationException(
                                 "SwaggerDocOptions section is missing in appsettings.json");

        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc(swaggerOptions.Name, new OpenApiInfo
            {
                Version = swaggerOptions.Version,
                Title = swaggerOptions.Title,
                Description = swaggerOptions.Description
            });

            foreach (var server in swaggerOptions.Servers)
            {
                opt.AddServer(new OpenApiServer
                {
                    Url = server.Url,
                    Description = server.Description
                });
            }

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });

            opt.EnableAnnotations();

            configure?.Invoke(opt);
        });
    }

    public static IApplicationBuilder UseSwaggerUi(this IApplicationBuilder app, IConfiguration configuration)
    {
        var swaggerOptions = configuration.GetSection("SwaggerDocOptions").Get<SwaggerDocOptions>()
                             ?? throw new InvalidOperationException(
                                 "SwaggerDocOptions section is missing in appsettings.json");

        app.UseSwagger();
        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint($"/swagger/{swaggerOptions.Name}/swagger.json",
                $"{swaggerOptions.Title} {swaggerOptions.Version}");
        });

        return app;
    }
}