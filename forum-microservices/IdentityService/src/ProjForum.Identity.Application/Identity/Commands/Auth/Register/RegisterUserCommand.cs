using MediatR;
using ProjForum.Identity.Application.DTOs;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.Register;

public record RegisterUserCommand(string UserName, string Email, string Password, IEnumerable<string>? Roles = null)
    : IRequest<OperationResultDto>;