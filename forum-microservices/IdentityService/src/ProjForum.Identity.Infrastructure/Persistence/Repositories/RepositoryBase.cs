using Microsoft.EntityFrameworkCore;
using ProjForum.BuildingBlocks.Domain.Interfaces;

namespace ProjForum.Identity.Infrastructure.Persistence.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class, IAggregateRoot
{
    protected readonly ApplicationDbContext DbContext;
    private readonly DbSet<T> _dbSet;

    protected RepositoryBase(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Delete(T entity) => _dbSet.Remove(entity);
}