using MediatR;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.Register;

public class RegisterUserCommandHandler(IAuthRepository authRepository)
    : IRequestHandler<RegisterUserCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var (success, error) = await authRepository.RegisterAsync(
            request.UserName,
            request.Email,
            request.Password,
            request.Roles,
            cancellationToken);

        if (!success)
            throw new InvalidOperationException(error);

        return new OperationResultDto(true, "User registered successfully");
    }
}
