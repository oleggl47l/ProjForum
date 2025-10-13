using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ProjForum.Identity.Infrastructure.Data.Extensions;

public static class DataSeedingExtensions
{
    public static void AddDataSeeding(this IServiceCollection services)
    {
        services.AddScoped<DatabaseSeeder>();
    }

    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}