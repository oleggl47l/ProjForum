using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;

public class RemoveRoleFromUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string RoleName { get; set; }
}