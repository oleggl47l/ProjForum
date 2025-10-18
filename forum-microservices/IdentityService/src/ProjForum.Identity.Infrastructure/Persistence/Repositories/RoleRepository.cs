using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;
using ProjForum.Identity.Infrastructure.Persistence.Entities;
using ProjForum.Identity.Infrastructure.Persistence.Mappers;

namespace ProjForum.Identity.Infrastructure.Persistence.Repositories;

public class RoleRepository(RoleManager<RoleEntity> roleManager) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await roleManager.FindByIdAsync(id.ToString());
        return entity?.ToDomain();
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await roleManager.FindByNameAsync(name);
        return entity?.ToDomain();
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        return entities.Select(r => r.ToDomain()).ToList();
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        var entity = new RoleEntity { Id = role.Id.ToString(), Name = role.Name, IsActive = role.IsActive };
        await roleManager.CreateAsync(entity);
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        var entity = await roleManager.FindByIdAsync(role.Id.ToString());

        if (entity != null)
        {
            entity.UpdateFromDomain(role);
            await roleManager.UpdateAsync(entity);
        }
    }

    public async Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
    {
        var entity = await roleManager.FindByIdAsync(role.Id.ToString());

        if (entity != null)
            await roleManager.DeleteAsync(entity);
    }
}