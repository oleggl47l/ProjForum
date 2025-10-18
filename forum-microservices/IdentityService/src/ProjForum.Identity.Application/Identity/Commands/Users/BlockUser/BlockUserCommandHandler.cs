using MediatR;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.BlockUser;

public class BlockUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<BlockUserCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");
        
        user.Deactivate();

        await userRepository.BlockAsync(user, request.TimeInMinutes, cancellationToken);

        return new OperationResultDto(true, $"User blocked for {request.TimeInMinutes} minutes");
    }
}