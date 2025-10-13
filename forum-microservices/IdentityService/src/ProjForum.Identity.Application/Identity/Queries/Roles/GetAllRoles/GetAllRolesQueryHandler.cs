using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjForum.Identity.Application.Identity.Queries.Roles.GetAllRoles;

public class GetAllRolesQueryHandler(RoleManager<Domain.Entities.Role> roleManager)
    : IRequestHandler<GetAllRolesQuery, List<Domain.Entities.Role>>
{
    public async Task<List<Domain.Entities.Role>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return roles;
    }
}