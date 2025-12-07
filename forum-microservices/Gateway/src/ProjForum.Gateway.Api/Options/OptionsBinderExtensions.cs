namespace ProjForum.Gateway.Api.Options;

public static class OptionsBinderExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<CorsPoliticsOptions>()
            .Bind(config.GetSection("CorsPoliticsOptions"))
            .ValidateDataAnnotations();

        services.AddOptions<JwtAuthenticationOptions>()
            .Bind(config.GetSection("JwtAuthenticationOptions"))
            .ValidateDataAnnotations();

        return services;
    }
}