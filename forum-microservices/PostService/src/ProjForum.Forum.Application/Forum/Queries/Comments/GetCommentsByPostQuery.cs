using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Comments;

public class GetCommentsByPostQuery : IRequest<IEnumerable<CommentModel>>
{
    [Required] public Guid PostId { get; set; }
}