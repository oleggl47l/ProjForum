using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Users.DeleteUser;

public class DeleteUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
}