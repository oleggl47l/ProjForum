using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UnblockUser;

public class UnblockUserCommand : IRequest<Unit>
{
    public string UserId { get; set; }
}