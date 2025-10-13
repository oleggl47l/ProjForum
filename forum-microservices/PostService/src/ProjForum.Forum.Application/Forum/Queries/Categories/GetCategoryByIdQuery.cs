using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Categories;

public class GetCategoryByIdQuery : IRequest<CategoryModel>
{
    [Required] public Guid Id { get; init; }
}