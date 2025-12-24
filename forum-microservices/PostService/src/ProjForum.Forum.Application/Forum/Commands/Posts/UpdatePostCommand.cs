using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class UpdatePostCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Guid? AuthorId { get; set; }
    public string? CategoryName { get; set; }
    public bool? IsPublished { get; set; }
}