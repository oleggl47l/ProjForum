using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Categories;

public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryModel>>;