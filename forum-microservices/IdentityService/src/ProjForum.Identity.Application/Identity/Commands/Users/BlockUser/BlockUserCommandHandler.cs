using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Interfaces;

namespace ProjForum.Identity.Application.Identity.Commands.Users.BlockUser;

public class BlockUserCommandHandler(
    UserManager<Domain.Entities.User> userManager,
    IUserService userService) : IRequestHandler<BlockUserCommand, Unit>
{
    public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException($"User with ID {request.UserId} not found");

        var lockoutEndTime = DateTime.UtcNow.AddMinutes(request.TimeInMinutes);

        user.Active = false;
        await userManager.UpdateAsync(user);

        await userManager.SetLockoutEnabledAsync(user, true);
        await userManager.SetLockoutEndDateAsync(user, lockoutEndTime);

        await userService.NotifyUserStatusChanged(user.Id);

        return Unit.Value;
    }
}