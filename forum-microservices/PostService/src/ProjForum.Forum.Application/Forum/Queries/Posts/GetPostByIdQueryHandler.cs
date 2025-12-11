using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetPostByIdQueryHandler(IPostRepository postRepository)
    : IRequestHandler<GetPostByIdQuery, PostModel?>
{
    public async Task<PostModel?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetPostWithCategoryAndTagsAsync(request.Id);
        return post is null
            ? null
            : new PostModel
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
            };
    }
}