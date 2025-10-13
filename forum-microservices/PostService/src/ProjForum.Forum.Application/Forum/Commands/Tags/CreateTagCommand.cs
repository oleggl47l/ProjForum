using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Tags;

public class CreateTagCommand : IRequest<Unit>
{
    public string Name { get; set; }
}