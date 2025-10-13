using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class DeleteUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
}