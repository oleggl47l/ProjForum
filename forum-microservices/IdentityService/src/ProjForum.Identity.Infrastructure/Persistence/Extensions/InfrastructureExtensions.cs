using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Domain.Interfaces;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Entities;
using ProjForum.Identity.Infrastructure.Persistence.Repositories;
using ProjForum.Identity.Infrastructure.Security;

namespace ProjForum.Identity.Infrastructure.Persistence.Extensions;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PFIdentity"));
        });

        // Identity
        services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        // Security
        services.AddScoped<IJwtGenerator, JwtTokenGenerator>();

        // Messaging (RabbitMQ / MassTransit)
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });

            // x.AddRequestClient<UserStatusChangedEvent>();
        });

        // Seeding
        services.AddScoped<DatabaseSeeder>();
    }

    public static async Task UseIdentitySeedingAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}