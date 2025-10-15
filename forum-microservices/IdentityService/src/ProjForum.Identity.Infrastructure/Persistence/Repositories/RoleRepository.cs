using Microsoft.EntityFrameworkCore;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Mappers;

namespace ProjForum.Identity.Infrastructure.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext dbContext) : RepositoryBase<Role>(dbContext), IRoleRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

        return entity?.ToDomain();
    }
}