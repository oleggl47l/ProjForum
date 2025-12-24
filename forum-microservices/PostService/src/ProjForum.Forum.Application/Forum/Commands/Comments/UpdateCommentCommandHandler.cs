using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class UpdateCommentCommandHandler(ICommentRepository commentRepository)
    : IRequestHandler<UpdateCommentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(request.Id);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id {request.Id} not found.");

        if (request.PostId != null && request.PostId != Guid.Empty)
            comment.PostId = request.PostId.Value;

        if (request.AuthorId != null && request.AuthorId != Guid.Empty)
            comment.AuthorId = request.AuthorId.Value;

        if (!string.IsNullOrWhiteSpace(request.Content))
            comment.Content = request.Content;

        comment.UpdatedAt = DateTime.UtcNow;

        await commentRepository.Update(comment);

        return Unit.Value;
    }
}