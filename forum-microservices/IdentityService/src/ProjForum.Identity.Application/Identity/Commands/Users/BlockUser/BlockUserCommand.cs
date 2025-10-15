using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Users.BlockUser;

public record BlockUserCommand(Guid UserId, int TimeInMinutes) : IRequest<OperationResultDto>;