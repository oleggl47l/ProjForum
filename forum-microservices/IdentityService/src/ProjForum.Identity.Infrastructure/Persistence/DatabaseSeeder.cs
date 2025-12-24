using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjForum.Identity.Infrastructure.Persistence.Entities;

namespace ProjForum.Identity.Infrastructure.Persistence;

public class DatabaseSeeder(
    UserManager<UserEntity> userManager,
    RoleManager<RoleEntity> roleManager,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "User", "Moderator" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new RoleEntity
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    IsActive = true
                });
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var usersToSeed = new[]
        {
            new { Email = "admin@forum.com", UserName = "admin", Password = "Admin123!", Role = "Admin" },
            new { Email = "moderator@forum.com", UserName = "moderator", Password = "Moderator123!", Role = "Moderator" },
            new { Email = "user@forum.com", UserName = "user", Password = "User123!", Role = "User" },
            new { Email = "test@forum.com", UserName = "test", Password = "Test123!", Role = "User" }
        };

        foreach (var userInfo in usersToSeed)
        {
            await CreateUserIfNotExistsAsync(
                userInfo.Email,
                userInfo.UserName,
                userInfo.Password,
                userInfo.Role);
        }
    }

    private async Task CreateUserIfNotExistsAsync(string email, string userName, string password, string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            var user = new UserEntity
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                Active = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
            else
            {
                logger.LogWarning(
                    $"Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}