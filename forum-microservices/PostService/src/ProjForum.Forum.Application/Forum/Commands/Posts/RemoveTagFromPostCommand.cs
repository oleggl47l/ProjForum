using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class RemoveTagFromPostCommand : IRequest<Unit>
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
}