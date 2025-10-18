using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Users.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<OperationResultDto>;