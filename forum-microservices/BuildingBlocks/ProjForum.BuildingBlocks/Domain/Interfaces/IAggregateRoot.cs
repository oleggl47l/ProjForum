namespace ProjForum.BuildingBlocks.Domain.Interfaces;

// маркер/контракт для репозиториев
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> Events { get; }

    void ClearEvents();
}