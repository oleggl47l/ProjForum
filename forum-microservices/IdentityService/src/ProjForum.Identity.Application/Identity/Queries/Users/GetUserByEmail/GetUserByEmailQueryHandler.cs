using MediatR;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUserByEmail;

public class GetUserByEmailQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null) return null;

        var roles = await userRepository.GetRolesAsync(user, cancellationToken);
        return new UserDto(user.Id, user.UserName, user.Email, user.Active, user.AccessFailedCount, roles);
    }
}