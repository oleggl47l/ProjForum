using ProjForum.Forum.Domain.Entities;

namespace ProjForum.Forum.Domain.Interfaces;

public interface IPostRepository : IRepositoryBase<Post>
{
    Task<IEnumerable<Post>> GetPostsByCategoryAsync(Guid categoryId);

    Task<IEnumerable<Post>> GetPostsByTagAsync(Guid tagId);

    Task AddTagToPostAsync(Guid postId, Guid tagId);

    Task RemoveTagFromPostAsync(Guid postId, Guid tagId);
    Task<Post?> GetPostWithCategoryAndTagsAsync(Guid postId);
    Task<IEnumerable<Post>> GetAllPostsWithCategoryAndTagsAsync();
    Task<IEnumerable<Tag>> GetTagsByPostIdAsync(Guid postId);
    Task<IEnumerable<Post>> GetPostsByUserIdAsync(Guid userId);
}