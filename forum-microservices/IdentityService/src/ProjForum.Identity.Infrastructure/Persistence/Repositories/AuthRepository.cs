using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Interfaces;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Entities;
using ProjForum.Identity.Infrastructure.Persistence.Mappers;

namespace ProjForum.Identity.Infrastructure.Persistence.Repositories;

public class AuthRepository(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    RoleManager<RoleEntity> roleManager,
    IJwtGenerator jwtGenerator)
    : IAuthRepository
{
    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string userName, string email, string password,
        IEnumerable<string>? roles = null, CancellationToken cancellationToken = default)
    {
        var existingUserName = await userManager.FindByNameAsync(userName);
        if (existingUserName is not null)
            return (false, "Username already exists");

        var existingEmail = await userManager.FindByEmailAsync(email);
        if (existingEmail is not null)
            return (false, "Email already exists");

        var user = new UserEntity
        {
            UserName = userName,
            Email = email,
            Active = true,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        var assignedRoles = roles?.ToList() ?? ["User"];

        foreach (var role in assignedRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                return (false, $"Role '{role}' does not exist");

            await userManager.AddToRoleAsync(user, role);
        }

        return (true, string.Empty);
    }

    public async
        Task<(bool Success, string Token, string RefreshToken, DateTime RefreshTokenExpires, string ErrorMessage)>
        LoginAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
            return (false, string.Empty, string.Empty, DateTime.MinValue, "Invalid credentials");

        var result = await signInManager.PasswordSignInAsync(userName, password, false, true);
        if (!result.Succeeded)
            return result.IsLockedOut
                ? (false, string.Empty, string.Empty, DateTime.MinValue, "User is locked out")
                : (false, string.Empty, string.Empty, DateTime.MinValue, "Invalid credentials");

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtGenerator.GenerateToken(user.ToDomain(), roles);
        var (refreshToken, expires) = jwtGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = expires;
        await userManager.UpdateAsync(user);

        return (true, token, refreshToken, expires, string.Empty);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await signInManager.SignOutAsync();
    }
}