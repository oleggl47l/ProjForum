using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Infrastructure.Persistence.Entities;

namespace ProjForum.Identity.Infrastructure.Persistence.Mappers;

public static class UserMapper
{
    public static User ToDomain(this UserEntity entity)
        => User.FromPersistence(
            Guid.Parse(entity.Id),
            entity.UserName!,
            entity.Email!,
            entity.Active,
            entity.RefreshToken,
            entity.RefreshTokenExpires,
            entity.AccessFailedCount
        );

    public static void UpdateFromDomain(this UserEntity entity, User user)
    {
        entity.UserName = user.UserName;
        entity.Email = user.Email;
        entity.Active = user.Active;
        entity.RefreshToken = user.RefreshToken;
        entity.RefreshTokenExpires = user.RefreshTokenExpires;
        entity.AccessFailedCount = user.AccessFailedCount;
    }
}