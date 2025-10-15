using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UnblockUser;

public record UnblockUserCommand(Guid UserId) : IRequest<OperationResultDto>;