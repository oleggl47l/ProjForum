using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Exceptions;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UpdateUser;

public class UpdateUserCommandHandler(
    UserManager<Domain.Entities.User> userManager) : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException($"User with id {nameof(user)} could not be found.");

        if (!string.IsNullOrEmpty(request.UserName))
            user.UserName = request.UserName;

        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.NewPassword))
        {
            var checkPassword = await userManager.CheckPasswordAsync(user, request.OldPassword);
            if (!checkPassword)
                throw new UnauthorizedException("Old password is incorrect.");

            var passwordChangeResult =
                await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!passwordChangeResult.Succeeded)
                throw new ValidationException(passwordChangeResult.Errors.Select(e => e.Description));
        }

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new InvalidOperationException("Failed to update user.");

        return true;
    }
}