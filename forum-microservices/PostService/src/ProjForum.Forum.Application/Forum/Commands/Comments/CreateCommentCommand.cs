using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class CreateCommentCommand : IRequest<Unit>
{
    [Required] public Guid PostId { get; set; }
    [Required] public string Content { get; set; }
}