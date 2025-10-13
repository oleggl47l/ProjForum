using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    private readonly UserManager<ProjForum.Identity.Domain.Entities.User> _userManager;
    private readonly RoleManager<ProjForum.Identity.Domain.Entities.Role> _roleManager;

    public RemoveRoleFromUserCommandValidator(UserManager<ProjForum.Identity.Domain.Entities.User> userManager,
        RoleManager<ProjForum.Identity.Domain.Entities.Role> roleManager)
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