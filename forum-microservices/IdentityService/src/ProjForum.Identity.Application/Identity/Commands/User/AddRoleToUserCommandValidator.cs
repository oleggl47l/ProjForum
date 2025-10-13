using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.User;

public class AddRoleToUserCommandValidator : AbstractValidator<AddRoleToUserCommand>
{
    private readonly UserManager<ProjForum.Identity.Domain.Entities.User> _userManager;
    private readonly RoleManager<ProjForum.Identity.Domain.Entities.Role> _roleManager;

    public AddRoleToUserCommandValidator(UserManager<ProjForum.Identity.Domain.Entities.User> userManager,
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
            .MustAsync(UserDoesNotHaveRole)
            .WithMessage("User already has the specified role.");
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

    private async Task<bool> UserDoesNotHaveRole(AddRoleToUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null) return true;

        var roles = await _userManager.GetRolesAsync(user);
        return !roles.Contains(command.RoleName);
    }
}