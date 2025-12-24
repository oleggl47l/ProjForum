using FluentValidation;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreatePostCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title should not exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category is required.")
            .MustAsync(CategoryExists).WithMessage("Category does not exist.");

        RuleForEach(x => x.TagNames)
            .NotEmpty().WithMessage("Tag name cannot be empty.")
            .MaximumLength(100).WithMessage("Tag name should not exceed 100 characters.")
            .When(x => x.TagNames != null && x.TagNames.Any());

        RuleFor(x => x.TagNames)
            .Must((_, tagNames) => AreTagsUnique(tagNames))
            .WithMessage("Tag names must be unique.");
    }

    private async Task<bool> CategoryExists(string categoryName, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryByNameAsync(categoryName);
        return category != null;
    }

    private bool AreTagsUnique(IEnumerable<string>? tagNames)
    {
        if (tagNames == null)
            return true;

        var distinctTagNames = tagNames.Distinct().ToList();
        return distinctTagNames.Count() == tagNames.Count();
    }
}