using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Domain.Interfaces;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Repositories;
using ProjForum.Identity.Infrastructure.Security;

namespace ProjForum.Identity.Infrastructure.Persistence.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PFIdentity"));
        });

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        // Security
        services.AddScoped<IJwtGenerator, JwtTokenGenerator>();

        // Seeding
        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    public static async Task UseIdentitySeedingAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}