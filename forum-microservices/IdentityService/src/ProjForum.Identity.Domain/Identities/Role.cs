using ProjForum.BuildingBlocks.Domain;

namespace ProjForum.Identity.Domain.Identities;

public sealed class Role : AggregateRoot<Guid>
{
    private Role(Guid id, string name, bool isActive) : base(id)
    {
        Name = name;
        IsActive = isActive;
    }

    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public static Role Create(string name, bool isActive = true)
        => new(Guid.NewGuid(), name, isActive);

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    
    public static Role FromPersistence(Guid id, string name, bool isActive)
        => new Role(id, name, isActive);
}