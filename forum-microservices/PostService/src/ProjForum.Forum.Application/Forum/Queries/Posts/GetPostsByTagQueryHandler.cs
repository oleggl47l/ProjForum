using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetPostsByTagQueryHandler(IPostRepository postRepository, ITagRepository tagRepository)
    : IRequestHandler<GetPostsByTagQuery, IEnumerable<SimplePostModel>>
{
    public async Task<IEnumerable<SimplePostModel>> Handle(GetPostsByTagQuery request,
        CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(request.TagId);
        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        var posts = await postRepository.GetPostsByTagAsync(request.TagId);
        return posts.Select(r => new SimplePostModel
        {
            Id = r.Id,
            Title = r.Title,
            Content = r.Content,
            AuthorId = r.AuthorId
        });
    }
}