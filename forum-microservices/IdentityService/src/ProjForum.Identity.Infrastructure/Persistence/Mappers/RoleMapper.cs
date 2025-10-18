using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Infrastructure.Persistence.Entities;

namespace ProjForum.Identity.Infrastructure.Persistence.Mappers;

public static class RoleMapper
{
    public static Role ToDomain(this RoleEntity entity)
        => Role.FromPersistence(
            Guid.Parse(entity.Id),
            entity.Name!,
            entity.IsActive
        );

    public static void UpdateFromDomain(this RoleEntity entity, Role role)
    {
        entity.Name = role.Name;
        entity.IsActive = role.IsActive;
    }
}