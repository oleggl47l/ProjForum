using FluentValidation;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdatePostCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Post ID is required.");


        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title should not exceed 200 characters.");

        RuleFor(x => x.CategoryName)
            .MustAsync(CategoryExists).WithMessage("Category does not exist.")
            .When(x => !string.IsNullOrWhiteSpace(x.CategoryName));
    }

    private async Task<bool> CategoryExists(string? categoryName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return true;

        var category = await _categoryRepository.GetCategoryByNameAsync(categoryName);
        return category != null;
    }
}