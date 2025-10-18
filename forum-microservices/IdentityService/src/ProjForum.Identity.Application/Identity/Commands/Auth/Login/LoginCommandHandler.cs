using MediatR;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.Auth;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.Login;

public class LoginCommandHandler(IAuthRepository authRepository)
    : IRequestHandler<LoginCommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, token, refreshToken, refreshExpires, error) =
            await authRepository.LoginAsync(request.UserName, request.Password, cancellationToken);

        if (!success)
        {
            if (error.Contains("locked out", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(error);

            if (error.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                error.Contains("Invalid credentials", StringComparison.OrdinalIgnoreCase))
                throw new KeyNotFoundException(error);

            throw new InvalidOperationException(error);
        }

        return new LoginResultDto(
            new OperationResultDto(true, "Login successful"),
            token,
            refreshToken,
            refreshExpires
        );
    }
}