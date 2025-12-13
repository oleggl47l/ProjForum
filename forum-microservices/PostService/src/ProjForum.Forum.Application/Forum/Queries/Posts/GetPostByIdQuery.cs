using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetPostByIdQuery : IRequest<PostModel?>
{
    [Required] public Guid Id { get; set; }
}