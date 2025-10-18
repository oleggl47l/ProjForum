using MediatR;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUserById;

public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null) return null;

        var roles = await userRepository.GetRolesAsync(user, cancellationToken);
        return new UserDto(user.Id, user.UserName, user.Email, user.Active, user.AccessFailedCount, roles);
    }
}