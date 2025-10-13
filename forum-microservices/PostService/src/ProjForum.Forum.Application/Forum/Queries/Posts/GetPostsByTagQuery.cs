using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetPostsByTagQuery : IRequest<IEnumerable<SimplePostModel>>
{
    [Required] public Guid TagId { get; set; }
}