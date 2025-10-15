using MediatR;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetPagedUsers;

public class GetPagedUsersHandler(IUserRepository userRepository) : IRequestHandler<GetPagedUsersQuery, PagedUsersDto>
{
    public async Task<PagedUsersDto> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, total) = await userRepository.GetPagedAsync(request.PageIndex, request.PageSize, cancellationToken);

        var dtoList = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userRepository.GetRolesAsync(user, cancellationToken);
            dtoList.Add(new UserDto(user.Id, user.UserName, user.Email, user.Active, user.AccessFailedCount, roles));
        }

        return new PagedUsersDto(dtoList, total, request.PageIndex, request.PageSize);
    }
}