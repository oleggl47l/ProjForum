using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetAllPostsQueryHandler(IPostRepository postRepository)
    : IRequestHandler<GetAllPostsQuery, IEnumerable<PostModel>>
{
    public async Task<IEnumerable<PostModel>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await postRepository.GetAllPostsWithCategoryAndTagsAsync();
        return posts.Select(post => new PostModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            IsPublished = post.IsPublished,
            Category = post.Category?.Name ?? string.Empty,
            TagNames = post.PostTags?.Select(rt => rt.Tag.Name).ToList()
        }).ToList();
    }
}