using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Entities;
using ProjForum.Identity.Infrastructure.Persistence.Mappers;

namespace ProjForum.Identity.Infrastructure.Persistence.Repositories;

public class UserRepository(UserManager<UserEntity> userManager, ApplicationDbContext dbContext)
    : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(id.ToString());
        return entity?.ToDomain();
    }

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

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await userManager.Users.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        return entities.Select(r => r.ToDomain()).ToList();
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = new UserEntity
        {
            Id = user.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            Active = user.Active,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpires = user.RefreshTokenExpires,
            AccessFailedCount = user.AccessFailedCount
        };

        await userManager.CreateAsync(entity);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());

        if (entity != null)
        {
            entity.UpdateFromDomain(user);
            await userManager.UpdateAsync(entity);
        }
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());

        if (entity != null)
            await userManager.DeleteAsync(entity);
    }

    public async Task BlockAsync(User user, int timeInMinutes, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());

        if (entity != null)
        {
            entity.Active = false;
            await userManager.SetLockoutEnabledAsync(entity, true);
            await userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.UtcNow.AddMinutes(timeInMinutes));
            await userManager.UpdateAsync(entity);
        }
    }

    public async Task UnblockAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());

        if (entity != null)
        {
            entity.Active = true;
            await userManager.SetLockoutEndDateAsync(entity, null);
            await userManager.UpdateAsync(entity);
        }
    }

    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());

        if (entity != null) await userManager.AddToRoleAsync(entity, roleName);
    }

    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());

        if (entity != null) await userManager.RemoveFromRoleAsync(entity, roleName);
    }

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await userManager.FindByIdAsync(user.Id.ToString());
        if (entity is null)
            return [];

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

        return (items.Select(u => u.ToDomain()).ToList(), total);
    }

    public async Task<IReadOnlyList<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId.ToString(), cancellationToken);
        if (role is null) return new List<User>();

        var users = await userManager.GetUsersInRoleAsync(role.Name!);
        return users.Select(u => u.ToDomain()).ToList();
    }
}