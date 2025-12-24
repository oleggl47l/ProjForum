using FluentValidation;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Tags;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(ITagRepository tagRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters")
            .CustomAsync(async (name, context, _) =>
            {
                if (await tagRepository.TagExistsByNameAsync(name))
                    context.AddFailure($"Tag with name '{name}' already exists.");
            });
    }
}