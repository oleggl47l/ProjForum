using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Queries.User;

public class GetUsersByRoleIdQueryHandler(
    UserManager<ProjForum.Identity.Domain.Entities.User> userManager,
    RoleManager<ProjForum.Identity.Domain.Entities.Role> roleManager) : IRequestHandler<GetUsersByRoleIdQuery, IEnumerable<UserModel>>
{
    public async Task<IEnumerable<UserModel>> Handle(GetUsersByRoleIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId);
        if (role == null)
            throw new NotFoundException($"Role with ID {request.RoleId} not found.");

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

        return usersInRole.Select(user => new UserModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Active = user.Active
        });
    }
}