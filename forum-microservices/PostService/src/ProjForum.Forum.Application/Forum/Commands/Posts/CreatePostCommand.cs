using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class CreatePostCommand : IRequest<Unit>
{
    [Required] public string Title { get; set; }
    [Required] public string Content { get; set; }
    [Required] public string CategoryName { get; set; }
    public List<string>? TagNames { get; set; }
}