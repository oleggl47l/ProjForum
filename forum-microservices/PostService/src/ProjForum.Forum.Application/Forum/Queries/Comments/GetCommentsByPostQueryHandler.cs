using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Comments;

public class GetCommentsByPostQueryHandler(ICommentRepository commentRepository)
    : IRequestHandler<GetCommentsByPostQuery, IEnumerable<CommentModel>>
{
    public async Task<IEnumerable<CommentModel>> Handle(GetCommentsByPostQuery request,
        CancellationToken cancellationToken)
    {
        var comments = await commentRepository.GetCommentsByPostIdAsync(request.PostId);
        return comments.Select(comment => new CommentModel
        {
            Id = comment.Id,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Content = comment.Content
        }).ToList();
    }
}