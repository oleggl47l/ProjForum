using MediatR;
using ProjForum.Identity.Application.DTOs.User;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;