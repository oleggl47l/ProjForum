using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetAllUsers;

public class GetAllUsersQueryHandler(UserManager<Domain.Entities.User> userManager)
    : IRequestHandler<GetAllUsersQuery, IEnumerable<UserModel>>
{
    public async Task<IEnumerable<UserModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var response = new List<UserModel>();

        foreach (var user in users)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var userResponse = new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userRoles.ToList(),
                Active = user.Active
            };

            response.Add(userResponse);
        }

        return response;
    }
}