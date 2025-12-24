using Microsoft.EntityFrameworkCore;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Infrastructure.Data.Repositories;

public class RepositoryBase<TEntity>(DbContext context) : IRepositoryBase<TEntity>
    where TEntity : class
{
    protected readonly DbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task Update(TEntity entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task Remove(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }
}