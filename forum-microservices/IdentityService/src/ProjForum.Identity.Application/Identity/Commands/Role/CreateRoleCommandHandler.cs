using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.Role;

public class CreateRoleCommandHandler(RoleManager<ProjForum.Identity.Domain.Entities.Role> roleManager) : IRequestHandler<CreateRoleCommand, Unit>
{
    public async Task<Unit> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new ProjForum.Identity.Domain.Entities.Role
        {
            Name = request.Name,
            IsActive = request.IsActive,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        
        var result = await roleManager.CreateAsync(role);
        
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to create role");
        
        return Unit.Value;
    }
}