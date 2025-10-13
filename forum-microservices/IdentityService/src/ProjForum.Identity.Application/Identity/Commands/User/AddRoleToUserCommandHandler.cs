using ProjForum.Identity.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class AddRoleToUserCommandHandler(
    UserManager<ProjForum.Identity.Domain.Entities.User> userManager,
    RoleManager<ProjForum.Identity.Domain.Entities.Role> roleManager)
    : IRequestHandler<AddRoleToUserCommand, bool>
{
    public async Task<bool> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
            throw new NotFoundException("Role does not exist");

        var result = await userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to add role to user");

        return result.Succeeded;
    }
}