using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class AddRoleToUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string RoleName { get; set; }
}