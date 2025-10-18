using ProjForum.BuildingBlocks.Domain.Interfaces;

namespace ProjForum.BuildingBlocks.Domain;

// корень агрегата, хранит доменные события, инварианты и методы для изменения состояния
// Агрегат - это группа связанных объектов, которые рассматриваются как единое целое. AggregateRoot - главный объект в этой группе.
public abstract class AggregateRoot<TId>(TId id) : Entity<TId>(id), IAggregateRoot
    where TId : notnull
{
    public IReadOnlyCollection<IDomainEvent> Events => [.. _events];

    private readonly List<IDomainEvent> _events = [];

    public void ClearEvents() => _events.Clear();

    protected void AddEvent<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent => _events.Add(@event);
}