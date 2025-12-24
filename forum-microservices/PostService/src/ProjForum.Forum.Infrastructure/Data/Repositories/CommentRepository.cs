using Microsoft.EntityFrameworkCore;
using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Infrastructure.Data.Repositories;

public class CommentRepository(ApplicationDbContext context)
    : RepositoryBase<Comment>(context), ICommentRepository
{
    public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId)
    {
        return await DbSet
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(Guid authorId)
    {
        return await DbSet
            .Where(c => c.AuthorId == authorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}