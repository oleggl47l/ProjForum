using Microsoft.EntityFrameworkCore;
using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Infrastructure.Data.Repositories;

public class CategoryRepository(ApplicationDbContext context) : RepositoryBase<Category>(context), ICategoryRepository
{
    public async Task<bool?> CategoryExistsByNameAsync(string name)
    {
        return await DbSet.AnyAsync(c => c.Name == name);
    }
    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Name == name);
    }
}