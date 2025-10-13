using ProjForum.Forum.Domain.Entities;

namespace ProjForum.Forum.Domain.Interfaces;

public interface ICategoryRepository : IRepositoryBase<Category>
{
    Task<bool?> CategoryExistsByNameAsync(string name);
    Task<Category?> GetCategoryByNameAsync(string name);
}