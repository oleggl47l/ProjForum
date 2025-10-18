using FluentValidation;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("RoleId is required.");

        RuleFor(command => command.Name)
            .MaximumLength(20)
            .WithMessage("Role name cannot exceed 20 characters.");
    }
}