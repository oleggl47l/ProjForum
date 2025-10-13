using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Interfaces;

namespace ProjForum.Identity.Application.Identity.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(UserManager<Domain.Entities.User> userManager, IUserService userService)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} could not be found.");

        var result = await userManager.DeleteAsync(user);
        await userService.NotifyUserDeleted(user.Id);
        return result.Succeeded;
    }
}