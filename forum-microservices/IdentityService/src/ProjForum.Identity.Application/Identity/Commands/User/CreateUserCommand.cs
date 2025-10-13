using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class CreateUserCommand : IRequest<Unit>
{
    [Required] public string UserName { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public List<string> RoleNames { get; set; }
}