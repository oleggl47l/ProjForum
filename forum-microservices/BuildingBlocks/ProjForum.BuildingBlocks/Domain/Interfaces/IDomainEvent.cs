using MediatR;

namespace ProjForum.BuildingBlocks.Domain.Interfaces;

// маркер для событий домена
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}