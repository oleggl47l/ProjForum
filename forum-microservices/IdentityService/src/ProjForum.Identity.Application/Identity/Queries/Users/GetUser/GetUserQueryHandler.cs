using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUser;

public class GetUserQueryHandler(
    UserManager<Domain.Entities.User> userManager) : IRequestHandler<GetUserQuery, UserModel>
{
    public async Task<UserModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} not found");

        var userRoles = await userManager
            .GetRolesAsync(user);

        return new UserModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = userRoles.ToList(),
            Active = user.Active
        };
    }
}