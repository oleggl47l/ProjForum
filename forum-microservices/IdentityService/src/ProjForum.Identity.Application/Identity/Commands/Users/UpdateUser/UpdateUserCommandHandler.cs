using MediatR;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, UpdateUserResultDto>
{
    public async Task<UpdateUserResultDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");

        var existingByEmail = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingByEmail is not null && existingByEmail.Id != user.Id)
            throw new InvalidOperationException("Email already in use");

        var existingByUserName = await userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (existingByUserName is not null && existingByUserName.Id != user.Id)
            throw new InvalidOperationException("Username already in use");

        user = User.FromPersistence(user.Id, request.UserName, request.Email, user.Active,
            user.RefreshToken, user.RefreshTokenExpires, user.AccessFailedCount);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await userRepository.GetRolesAsync(user, cancellationToken);
        var dto = new UserDto(user.Id, user.UserName, user.Email, user.Active, user.AccessFailedCount, roles);

        return new UpdateUserResultDto(new OperationResultDto(true, "User updated"), dto);
    }
}