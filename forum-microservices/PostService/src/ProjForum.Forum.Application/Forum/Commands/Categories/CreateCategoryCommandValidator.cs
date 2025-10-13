using FluentValidation;

namespace ProjForum.Forum.Application.Forum.Commands.Categories;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        RuleFor(command => command.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}