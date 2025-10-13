namespace ProjForum.Forum.Domain.Interfaces;

public interface IRepositoryBase<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task Update(TEntity entity);
    Task Remove(Guid id);
}