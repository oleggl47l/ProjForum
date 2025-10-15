using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUsersByRoleId;

public class GetUsersByRoleIdHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersByRoleIdQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersByRoleIdQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetByRoleIdAsync(request.RoleId, cancellationToken);

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userRepository.GetRolesAsync(user, cancellationToken);
            result.Add(new UserDto(user.Id, user.UserName, user.Email, user.Active, user.AccessFailedCount, roles));
        }

        return result;
    }
}