using ProjForum.BuildingBlocks.Domain;
using ProjForum.BuildingBlocks.Domain.Events;

namespace ProjForum.Identity.Domain.Identities;

public sealed class User : AggregateRoot<Guid>
{
    private User(Guid id, string userName, string email, bool active) : base(id)
    {
        UserName = userName;
        Email = email;
        Active = active;
    }

    public string UserName { get; private set; }
    public string Email { get; private set; }
    public bool Active { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; private set; }
    public int AccessFailedCount { get; private set; }

    public static User Create(string userName, string email)
        => new(Guid.NewGuid(), userName, email, true);

    public void Activate()
    {
        Active = true;
        AddEvent(new UserStatusChangedDomainEvent(Id, Active, DateTime.UtcNow));
    }

    public void Deactivate()
    {
        Active = false;
        AddEvent(new UserStatusChangedDomainEvent(Id, Active, DateTime.UtcNow));
    }

    public void MarkAsDeleted()
    {
        AddEvent(new UserDeletedDomainEvent(Id, DateTime.UtcNow));
    }

    public static User FromPersistence(Guid id, string userName, string email, bool active,
        string refreshToken, DateTime refreshTokenExpires, int accessFailedCount)
    {
        var user = new User(id, userName, email, active)
        {
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshTokenExpires,
            AccessFailedCount = accessFailedCount
        };
        return user;
    }
}