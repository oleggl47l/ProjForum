using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class UpdateCommentCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; set; }
    public Guid? PostId { get; set; }
    public Guid? AuthorId { get; set; }
    public string? Content { get; set; }
}