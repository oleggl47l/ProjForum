using MediatR;
using ProjForum.Identity.Application.DTOs.User;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUsersByRoleId;

public record GetUsersByRoleIdQuery(Guid RoleId) : IRequest<IReadOnlyList<UserDto>>;
