using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class UpdatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository) : IRequestHandler<UpdatePostCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.Id);
        if (post == null)
            throw new KeyNotFoundException($"Post with id {request.Id} not found.");
        
        if (!string.IsNullOrWhiteSpace(request.CategoryName))
        {
            var category = await categoryRepository.GetCategoryByNameAsync(request.CategoryName);
            if (category == null)
                throw new KeyNotFoundException($"Category with name {request.CategoryName} not found.");

            post.CategoryId = category.Id;
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
            post.Title = request.Title;
        
        if (!string.IsNullOrWhiteSpace(request.Content))
            post.Content = request.Content;
        
        if (request.AuthorId != null && request.AuthorId != Guid.Empty)
            post.AuthorId = request.AuthorId.Value;
        
        if (request.IsPublished.HasValue)
            post.IsPublished = request.IsPublished.Value;
        
        post.UpdatedAt = DateTime.UtcNow;
        
        await postRepository.Update(post);

        return Unit.Value;
    }
}