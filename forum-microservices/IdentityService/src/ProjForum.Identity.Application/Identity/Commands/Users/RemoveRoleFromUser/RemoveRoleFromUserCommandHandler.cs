using MediatR;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;

public class RemoveRoleFromUserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    : IRequestHandler<RemoveRoleFromUserCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");

        _ = await roleRepository.GetByNameAsync(request.RoleName, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{request.RoleName}' not found");

        var userRoles = await userRepository.GetRolesAsync(user, cancellationToken);
        if (!userRoles.Contains(request.RoleName))
            throw new InvalidOperationException($"User does not have role '{request.RoleName}'");

        if (userRoles.Count <= 1)
            throw new InvalidOperationException("User must have at least one role");

        await userRepository.RemoveFromRoleAsync(user, request.RoleName, cancellationToken);
        return new OperationResultDto(true, $"Role '{request.RoleName}' removed from user");
    }
}