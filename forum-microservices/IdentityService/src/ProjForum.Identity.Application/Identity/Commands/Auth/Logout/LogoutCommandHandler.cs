using MediatR;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.Logout;

public class LogoutCommandHandler(IAuthRepository authRepository)
    : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await authRepository.LogoutAsync(cancellationToken);
        return Unit.Value;
    }
}