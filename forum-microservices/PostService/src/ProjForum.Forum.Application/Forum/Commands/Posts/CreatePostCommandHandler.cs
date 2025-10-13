using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class CreatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    ITagRepository tagRepository,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreatePostCommand, Unit>
{
    public async Task<Unit> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User is not authorized.");

        var userId = Guid.Parse(userIdClaim.Value);

        var category = await categoryRepository.GetCategoryByNameAsync(request.CategoryName);
        if (category == null)
            throw new KeyNotFoundException($"Category {category} not found");

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = userId,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPublished = true
        };

        var existingTags = await tagRepository.GetAllAsync();
        var postTags = new List<PostTag>();

        if (request.TagNames != null && request.TagNames.Any())
        {
            foreach (var tagName in request.TagNames)
            {
                var tag = existingTags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
                if (tag != null)
                    postTags.Add(new PostTag { TagId = tag.Id, Post = post });
                else
                {
                    tag = new Tag { Name = tagName };
                    await tagRepository.AddAsync(tag);
                    postTags.Add(new PostTag { TagId = tag.Id, Post = post });
                }
            }
        }

        post.PostTags = postTags;

        await postRepository.AddAsync(post);

        return Unit.Value;
    }
}