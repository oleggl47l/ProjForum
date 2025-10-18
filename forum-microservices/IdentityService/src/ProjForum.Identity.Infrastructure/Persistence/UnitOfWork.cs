using MediatR;
using ProjForum.BuildingBlocks.Domain.Interfaces;

namespace ProjForum.Identity.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext dbContext, IMediator mediator) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(cancellationToken);

        return result;    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action();
            await dbContext.SaveChangesAsync(cancellationToken);
            
            await DispatchDomainEventsAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await action();
            await dbContext.SaveChangesAsync(cancellationToken);
            
            await DispatchDomainEventsAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var entities = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.Events.Any())
            .ToList();

        var domainEvents = entities.SelectMany(e => e.Events).ToList();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        entities.ForEach(e => e.ClearEvents());
    }
}