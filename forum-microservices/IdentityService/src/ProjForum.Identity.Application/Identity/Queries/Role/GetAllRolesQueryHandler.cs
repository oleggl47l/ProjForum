using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjForum.Identity.Application.Identity.Queries.Role;

public class GetAllRolesQueryHandler(RoleManager<ProjForum.Identity.Domain.Entities.Role> roleManager)
    : IRequestHandler<GetAllRolesQuery, List<ProjForum.Identity.Domain.Entities.Role>>
{
    public async Task<List<ProjForum.Identity.Domain.Entities.Role>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
       
        return roles;
    }
}