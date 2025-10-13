using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class DeletePostCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; set; }
}