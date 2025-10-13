using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Interfaces;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UnblockUser;

public class UnblockUserCommandHandler(
    UserManager<Domain.Entities.User> userManager,
    IUserNotificationService userNotificationService) : IRequestHandler<UnblockUserCommand, Unit>
{
    public async Task<Unit> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow);

        user.Active = true;
        await userManager.UpdateAsync(user);

        await userNotificationService.NotifyUserStatusChanged(user.Id);

        return Unit.Value;
    }
}