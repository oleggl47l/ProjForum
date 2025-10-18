using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.Logout;

public record LogoutCommand : IRequest<Unit>;