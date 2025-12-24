using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class DeleteCommentCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; set; }
}