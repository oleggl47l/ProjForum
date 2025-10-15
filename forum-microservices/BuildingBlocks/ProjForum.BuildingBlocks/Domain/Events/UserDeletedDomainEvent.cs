using ProjForum.BuildingBlocks.Domain.Interfaces;

namespace ProjForum.BuildingBlocks.Domain.Events;

public record UserDeletedDomainEvent(Guid UserId, DateTime DeletedAt) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}