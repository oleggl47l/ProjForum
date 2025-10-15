using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Domain.Identities;

namespace ProjForum.Identity.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    Task BlockAsync(User user, int timeInMinutes, CancellationToken cancellationToken = default);
    Task UnblockAsync(User user, CancellationToken cancellationToken = default);

    Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken = default);
    Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken = default);

    Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<User> Users, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}