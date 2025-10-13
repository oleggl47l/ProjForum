using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class UpdateUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}