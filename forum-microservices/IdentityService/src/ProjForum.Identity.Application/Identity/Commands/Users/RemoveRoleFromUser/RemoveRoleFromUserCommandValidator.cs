using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;

public class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public RemoveRoleFromUserCommandValidator(UserManager<Domain.Entities.User> userManager,
        RoleManager<Domain.Entities.Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .MustAsync(UserExists).WithMessage("User does not exist.");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("RoleName is required.")
            .MustAsync(RoleExists).WithMessage("Role does not exist.");

        RuleFor(x => x)
            .MustAsync(UserHasRole)
            .WithMessage("User is not assigned to the specified role.");

        RuleFor(x => x)
            .MustAsync(UserHasAtLeastOneRoleAfterRemoval)
            .WithMessage("User must have at least one role after removal.");
    }

    private async Task<bool> UserExists(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null;
    }

    private async Task<bool> RoleExists(string roleName, CancellationToken cancellationToken)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    private async Task<bool> UserHasRole(RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null) return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains(command.RoleName);
    }

    private async Task<bool> UserHasAtLeastOneRoleAfterRemoval(RemoveRoleFromUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null) return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Count > 1 || !roles.Contains(command.RoleName);
    }
}