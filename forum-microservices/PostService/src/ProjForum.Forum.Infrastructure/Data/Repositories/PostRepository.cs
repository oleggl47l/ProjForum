using Microsoft.EntityFrameworkCore;
using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Infrastructure.Data.Repositories;

public class PostRepository(ApplicationDbContext context)
    : RepositoryBase<Post>(context), IPostRepository
{
    public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(Guid categoryId)
    {
        return await DbSet
            .Where(r => r.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPostsByTagAsync(Guid tagId)
    {
        return await Context.Set<PostTag>()
            .Where(rt => rt.TagId == tagId)
            .Select(rt => rt.Post)
            .ToListAsync();
    }

    public async Task AddTagToPostAsync(Guid postId, Guid tagId)
    {
        var post = await GetByIdAsync(postId);
        if (post == null) return;

        var tag = await Context.Set<Tag>().FindAsync(tagId);
        if (tag == null) return;

        var postTag = new PostTag
        {
            PostId = postId,
            TagId = tagId
        };

        await Context.Set<PostTag>().AddAsync(postTag);
        await Context.SaveChangesAsync();
    }

    public async Task RemoveTagFromPostAsync(Guid postId, Guid tagId)
    {
        var postTag = await Context.Set<PostTag>()
            .FirstOrDefaultAsync(rt => rt.PostId == postId && rt.TagId == tagId);
        if (postTag == null) return;

        Context.Set<PostTag>().Remove(postTag);
        await Context.SaveChangesAsync();
    }

    public async Task<Post?> GetPostWithCategoryAndTagsAsync(Guid postId)
    {
        return await DbSet
            .Include(r => r.Category)
            .Include(r => r.PostTags)
            .ThenInclude(rt => rt.Tag)
            .FirstOrDefaultAsync(r => r.Id == postId);
    }

    public async Task<IEnumerable<Post>> GetAllPostsWithCategoryAndTagsAsync()
    {
        return await DbSet
            .Include(r => r.Category)
            .Include(r => r.PostTags)
            .ThenInclude(rt => rt.Tag)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetTagsByPostIdAsync(Guid postId)
    {
        return await Context.Set<PostTag>()
            .Where(rt => rt.PostId == postId)
            .Select(rt => rt.Tag)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(Guid userId)
    {
        return await DbSet
            .Where(r => r.AuthorId == userId)
            .ToListAsync();
    }
}