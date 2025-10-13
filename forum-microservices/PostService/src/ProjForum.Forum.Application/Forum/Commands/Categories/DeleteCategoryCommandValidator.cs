using FluentValidation;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Categories;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    private readonly IPostRepository _postRepository;

    public DeleteCategoryCommandValidator(IPostRepository postRepository)
    {
        _postRepository = postRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty");

        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellationToken) => !await CategoryHasPostsAsync(id, cancellationToken))
            .WithMessage("Category cannot be deleted because it has associated posts.");
    }

    private async Task<bool> CategoryHasPostsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var posts = await _postRepository.GetPostsByCategoryAsync(categoryId);
        return posts.Any();
    }
}