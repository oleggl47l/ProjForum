using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.DeleteRole;

public record DeleteRoleCommand(Guid Id) : IRequest<OperationResultDto>;
