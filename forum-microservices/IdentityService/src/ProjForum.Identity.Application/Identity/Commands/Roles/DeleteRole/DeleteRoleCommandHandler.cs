using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.DeleteRole;

public class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRoleCommand, OperationResultDto>
{
    public async Task<OperationResultDto> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new KeyNotFoundException("Role not found");

        var usersWithRole = await userRepository.GetByRoleIdAsync(request.Id, cancellationToken);
        if (usersWithRole.Count > 0)
            throw new InvalidOperationException(
                "Impossible to delete a role because there are users who have this role.");

        roleRepository.Delete(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OperationResultDto(true, "Role deleted");
    }
}