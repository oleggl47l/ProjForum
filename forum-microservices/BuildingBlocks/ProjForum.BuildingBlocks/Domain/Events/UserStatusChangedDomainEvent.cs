using ProjForum.BuildingBlocks.Domain.Interfaces;

namespace ProjForum.BuildingBlocks.Domain.Events;

public record UserStatusChangedDomainEvent(Guid UserId, bool Active, DateTime ChangedAt) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}