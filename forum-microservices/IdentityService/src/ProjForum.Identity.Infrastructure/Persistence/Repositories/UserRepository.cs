using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Entities;
using ProjForum.Identity.Infrastructure.Persistence.Mappers;

namespace ProjForum.Identity.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext dbContext, UserManager<UserEntity> userManager)
    : RepositoryBase<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return entity?.ToDomain();
    }

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        return entity?.ToDomain();
    }

    public async Task BlockAsync(User user, int timeInMinutes, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());
        if (entity is not null)
        {
            entity.Active = false;
            await userManager.UpdateAsync(entity);

            var lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(timeInMinutes);
            await userManager.SetLockoutEnabledAsync(entity, true);
            await userManager.SetLockoutEndDateAsync(entity, lockoutEnd);
        }
    }

    public async Task UnblockAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());
        if (entity is not null)
        {
            entity.Active = true;
            await userManager.UpdateAsync(entity);
        }
    }

    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());
        if (entity is not null)
            await userManager.AddToRoleAsync(entity, roleName);
    }

    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());
        if (entity is not null)
            await userManager.RemoveFromRoleAsync(entity, roleName);
    }

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());
        if (entity is null) return new List<string>();

        return await userManager.GetRolesAsync(entity);
    }

    public async Task<(IReadOnlyList<User> Users, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = userManager.Users.AsNoTracking();

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items.Select(x => x.ToDomain()).ToList(), total);
    }

    public async Task<IReadOnlyList<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await DbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId.ToString(), cancellationToken);
        if (role is null) return new List<User>();

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);

        return usersInRole.Select(u => u.ToDomain()).ToList();
    }
}