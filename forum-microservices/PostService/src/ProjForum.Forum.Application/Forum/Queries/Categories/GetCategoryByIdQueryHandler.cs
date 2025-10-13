using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Categories;

public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, CategoryModel>
{
    public async Task<CategoryModel> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {request.Id} not found.");
        return new CategoryModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }
}