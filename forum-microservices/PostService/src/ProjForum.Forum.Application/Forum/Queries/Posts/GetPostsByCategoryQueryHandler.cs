using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetPostsByCategoryQueryHandler(IPostRepository postRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<GetPostsByCategoryQuery, IEnumerable<SimplePostModel>>
{
    public async Task<IEnumerable<SimplePostModel>> Handle(GetPostsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        var posts = await postRepository.GetPostsByCategoryAsync(category.Id);
        return posts.Select(r => new SimplePostModel
        {
            Id = r.Id,
            Title = r.Title,
            Content = r.Content,
            AuthorId = r.AuthorId
        });
    }
}