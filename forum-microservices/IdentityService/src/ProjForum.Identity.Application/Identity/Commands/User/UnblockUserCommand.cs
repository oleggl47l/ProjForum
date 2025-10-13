using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class UnblockUserCommand: IRequest<Unit>
{
    public string UserId { get; set; }
}