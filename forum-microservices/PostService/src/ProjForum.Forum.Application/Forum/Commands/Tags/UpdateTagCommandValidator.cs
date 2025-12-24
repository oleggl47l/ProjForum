using FluentValidation;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Tags;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator(ITagRepository tagRepository)
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be empty");
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters")
            .CustomAsync(async (name, context, _) =>
            {
                if (await tagRepository.TagExistsByNameAsync(name))
                    context.AddFailure($"Tag with name '{name}' already exists.");
            });
    }
}