using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.Users.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public CreateUserCommandValidator(UserManager<Domain.Entities.User> userManager,
        RoleManager<Domain.Entities.Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User Name is required.")
            .MaximumLength(20).WithMessage("User Name must not exceed 20 characters.")
            .MustAsync(IsUniqueUserName).WithMessage("User Name already exists.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format.")
            .MustAsync(IsUniqueEmail).WithMessage("Email already exists.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-ZА-Я]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-zа-я]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\W_]")
            .WithMessage("Password must contain at least one special character (e.g., !, @, #, $, etc.).");

        RuleFor(x => x.RoleNames)
            .NotEmpty().WithMessage("At least one role must be assigned.")
            .Must(roles => roles != null && roles.Any()).WithMessage("Role list cannot be empty.")
            .ForEach(roleName => roleName.MustAsync(IsValidRoleAsync).WithMessage("Role does not exist."));
    }

    private async Task<bool> IsUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        return await _userManager.FindByNameAsync(userName) == null;
    }

    private async Task<bool> IsUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return await _userManager.FindByEmailAsync(email) == null;
    }

    private async Task<bool> IsValidRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        return role != null;
    }
}