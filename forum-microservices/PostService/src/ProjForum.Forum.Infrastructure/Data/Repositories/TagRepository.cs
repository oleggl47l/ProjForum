using Microsoft.EntityFrameworkCore;
using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Infrastructure.Data.Repositories;

public class TagRepository(ApplicationDbContext context) : RepositoryBase<Tag>(context), ITagRepository
{
    public async Task<bool> TagExistsByNameAsync(string name)
    {
        return await DbSet.AnyAsync(t => t.Name == name);
    }
    
    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.Name == name);
    }
}