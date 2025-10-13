using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjForum.Identity.Application.Identity.Queries.Roles.GetRole;

public class GetRoleQueryHandler(RoleManager<Domain.Entities.Role> roleManager)
    : IRequestHandler<GetRoleQuery, Domain.Entities.Role>
{
    public async Task<Domain.Entities.Role> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
            throw new KeyNotFoundException($"Role with id {request.RoleId} not found");

        return role;
    }
}