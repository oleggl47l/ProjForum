using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Interfaces;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Auth.Login;

public class LoginQueryHandler(
    SignInManager<Domain.Entities.User> signInManager,
    UserManager<Domain.Entities.User> userManager,
    ILogger<LoginQueryHandler> logger,
    IJwtGenerator jwtTokenGenerator) : IRequestHandler<LoginQuery, LoginModel>
{
    public async Task<LoginModel> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserName)
                   ?? throw new NotFoundException($"User {request.UserName} not found.");

        var roles = await userManager.GetRolesAsync(user);

        if (user.AccessFailedCount > 2)
        {
            logger.LogError($"AccessFailedCount {user.AccessFailedCount}.");
            user.Active = false;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                logger.LogError($"\n\nFailed to update user {user.UserName} status to inactive.\n\n");
        }

        var signInResult = await signInManager.PasswordSignInAsync(request.UserName, request.Password, false, true);


        if (signInResult.IsLockedOut)
            throw new UserBlockedException(user.UserName, user.LockoutEnd.Value);

        if (!signInResult.Succeeded)
            throw new LoginException();

        var token = jwtTokenGenerator.GenerateToken(user, roles);
        var (refreshToken, expires) = jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = expires;

        await userManager.UpdateAsync(user);

        return new LoginModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = token
        };
    }
}