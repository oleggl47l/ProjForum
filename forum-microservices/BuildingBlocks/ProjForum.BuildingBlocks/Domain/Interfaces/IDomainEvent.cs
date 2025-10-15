namespace ProjForum.BuildingBlocks.Domain.Interfaces;

// маркер для событий домена
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}