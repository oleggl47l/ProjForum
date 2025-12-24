using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Categories;

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IPostRepository postRepository)
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {request.Id} not found.");

        var posts = await postRepository.GetPostsByCategoryAsync(request.Id);
        if (posts.Any())
            throw new InvalidOperationException("Cannot delete category because there are posts associated with it.");

        await categoryRepository.Remove(request.Id);
        return Unit.Value;
    }
}