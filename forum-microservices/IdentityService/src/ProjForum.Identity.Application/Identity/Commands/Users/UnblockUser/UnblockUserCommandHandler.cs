using MediatR;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UnblockUser;

public class UnblockUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UnblockUserCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");

        user.Activate();

        await userRepository.UnblockAsync(user, cancellationToken);
        return new OperationResultDto(true, "User unblocked");
    }
}