using MediatR;
using ProjForum.Identity.Application.DTOs.Auth;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.Login;

public record LoginCommand(string UserName, string Password) : IRequest<LoginResultDto>;