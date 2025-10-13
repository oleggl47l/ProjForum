using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Exceptions;

namespace ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;

public class RemoveRoleFromUserCommandHandler(
    UserManager<Domain.Entities.User> userManager,
    RoleManager<Domain.Entities.Role> roleManager)
    : IRequestHandler<RemoveRoleFromUserCommand, bool>
{
    public async Task<bool> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} not found");

        var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
            throw new NotFoundException($"Role with name {request.RoleName} not found");

        var result = await userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to remove role to user");

        return result.Succeeded;
    }
}