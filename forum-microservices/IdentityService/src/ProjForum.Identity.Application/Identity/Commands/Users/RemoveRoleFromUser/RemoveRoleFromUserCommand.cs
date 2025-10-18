using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;

public record RemoveRoleFromUserCommand(Guid UserId, string RoleName) : IRequest<OperationResultDto>;