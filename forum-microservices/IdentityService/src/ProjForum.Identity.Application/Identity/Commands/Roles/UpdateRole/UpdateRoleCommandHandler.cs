using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.Role;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.UpdateRole;

public class UpdateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoleCommand, UpdateRoleResultDto>
{
    public async Task<UpdateRoleResultDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new KeyNotFoundException("Role not found");

        var existingByName = await roleRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingByName is not null && existingByName.Id != role.Id)
            throw new InvalidOperationException("Role name already in use");

        role = Role.FromPersistence(role.Id, request.Name, request.IsActive);

        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new RoleDto(role.Id, role.Name, role.IsActive);
        return new UpdateRoleResultDto(new OperationResultDto(true, "Role updated"), dto);
    }
}