using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Categories;

public class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryModel>>
{
    public async Task<IEnumerable<CategoryModel>> Handle(GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync();

        var categoryModels = categories.Select(category => new CategoryModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });

        return categoryModels;
    }
}