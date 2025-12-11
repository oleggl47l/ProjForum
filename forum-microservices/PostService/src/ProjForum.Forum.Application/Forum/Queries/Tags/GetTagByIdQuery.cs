using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Tags;

public class GetTagByIdQuery : IRequest<TagModel?>
{
    [Required] public Guid Id { get; set; }
}