using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Comments;

public class GetCommentByIdQuery : IRequest<CommentModel?>
{
    [Required] public Guid Id { get; set; }
}