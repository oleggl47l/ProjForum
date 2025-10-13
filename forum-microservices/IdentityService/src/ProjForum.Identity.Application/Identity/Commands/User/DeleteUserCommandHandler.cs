using ProjForum.Identity.Domain.Exceptions;
using ProjForum.Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class DeleteUserCommandHandler(UserManager<ProjForum.Identity.Domain.Entities.User> userManager, IUserService userService)
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