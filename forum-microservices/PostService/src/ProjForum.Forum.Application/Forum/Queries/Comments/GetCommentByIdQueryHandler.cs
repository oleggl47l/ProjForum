using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Comments;

public class GetCommentByIdQueryHandler(ICommentRepository commentRepository)
    : IRequestHandler<GetCommentByIdQuery, CommentModel?>
{
    public async Task<CommentModel?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(request.Id);
        return comment is null
            ? null
            : new CommentModel
            {
                Id = comment.Id,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Content = comment.Content
            };
    }
}