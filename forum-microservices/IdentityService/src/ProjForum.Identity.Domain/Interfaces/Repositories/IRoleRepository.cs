using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Domain.Identities;

namespace ProjForum.Identity.Domain.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}