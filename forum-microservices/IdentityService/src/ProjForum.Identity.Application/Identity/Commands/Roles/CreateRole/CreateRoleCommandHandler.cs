using MediatR;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.Role;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.CreateRole;

public class CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoleCommand, CreateRoleResultDto>
{
    public async Task<CreateRoleResultDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var existing = await roleRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Role '{request.Name}' already exists");

        var role = Role.Create(request.Name, request.IsActive);

        await roleRepository.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new RoleDto(role.Id, role.Name, role.IsActive);
        return new CreateRoleResultDto(new OperationResultDto(true, "Role created successfully"), dto);
    }
}