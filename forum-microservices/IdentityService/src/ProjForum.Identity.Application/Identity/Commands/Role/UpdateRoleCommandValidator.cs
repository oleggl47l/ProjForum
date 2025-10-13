using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.Role;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(RoleManager<ProjForum.Identity.Domain.Entities.Role> roleManager)
    {
        RuleFor(command => command.RoleId)
            .NotEmpty()
            .WithMessage("RoleId is required.");

        RuleFor(command => command.Name)
            .MaximumLength(20)
            .WithMessage("Role name cannot exceed 20 characters.")
            .MustAsync(async (name, _) =>
            {
                if (string.IsNullOrEmpty(name)) return true;
                var existingRole = await roleManager.FindByNameAsync(name);
                return existingRole == null;
            })
            .WithMessage("A role with the given name already exists.");

        RuleFor(command => command.IsActive)
            .Must(value => value == null || value is bool)
            .WithMessage("IsActive must be a boolean value.");
    }
}