using MediatR;
using ProjForum.Identity.Application.DTOs.User;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string UserName,
    string Email
) : IRequest<UpdateUserResultDto>;