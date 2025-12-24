using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class CreateCommentCommandHandler(
    ICommentRepository commentRepository,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateCommentCommand, Unit>
{
    public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User is not authorized.");

        var userId = Guid.Parse(userIdClaim.Value);

        var comment = new Comment
        {
            PostId = request.PostId,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Content = request.Content
        };

        await commentRepository.AddAsync(comment);

        return Unit.Value;
    }
}