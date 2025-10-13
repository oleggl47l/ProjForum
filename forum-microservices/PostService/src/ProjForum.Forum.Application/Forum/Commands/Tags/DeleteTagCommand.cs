using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Tags;

public class DeleteTagCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; init; }
}