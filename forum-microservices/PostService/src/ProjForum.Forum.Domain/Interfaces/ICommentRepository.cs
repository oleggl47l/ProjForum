using ProjForum.Forum.Domain.Entities;

namespace ProjForum.Forum.Domain.Interfaces;

public interface ICommentRepository : IRepositoryBase<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(Guid authorId);

    // TODO: интересная идея для лк пользователя
    // Task<IEnumerable<Post>> GetPostsCommentedByUserAsync(Guid userId); 
}