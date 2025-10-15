using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ProjForum.Identity.Domain.Identities;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(RoleManager<Role> roleManager)
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(20)
            .WithMessage("Role name cannot exceed 20 characters.");
    }
}