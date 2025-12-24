using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class DeleteCommentCommandHandler(ICommentRepository commentRepository)
    : IRequestHandler<DeleteCommentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(request.Id);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id {request.Id} not found.");

        await commentRepository.Remove(comment.Id);
        return Unit.Value;
    }
}