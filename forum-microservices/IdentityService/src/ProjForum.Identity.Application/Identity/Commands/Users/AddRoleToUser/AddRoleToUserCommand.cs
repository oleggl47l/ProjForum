using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Users.AddRoleToUser;

public record AddRoleToUserCommand(Guid UserId, string RoleName) : IRequest<OperationResultDto>;
