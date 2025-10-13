using ProjForum.Forum.Domain.Entities;

namespace ProjForum.Forum.Domain.Interfaces;

public interface ITagRepository : IRepositoryBase<Tag>
{
    Task<bool> TagExistsByNameAsync(string name);
    Task<Tag?> GetTagByNameAsync(string name);
}