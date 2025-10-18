using MediatR;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.AddRoleToUser;

public class AddRoleToUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<AddRoleToUserCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");

        _ = await roleRepository.GetByNameAsync(request.RoleName, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{request.RoleName}' not found");

        var userRoles = await userRepository.GetRolesAsync(user, cancellationToken);
        if (userRoles.Contains(request.RoleName))
            throw new InvalidOperationException($"User already has role '{request.RoleName}'");

        await userRepository.AddToRoleAsync(user, request.RoleName, cancellationToken);

        return new OperationResultDto(true, $"Role '{request.RoleName}' added to user");
    }
}