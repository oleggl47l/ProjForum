using MediatR;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");

        user.MarkAsDeleted();
        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OperationResultDto(true, "User deleted");
    }
}