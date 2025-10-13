using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Categories;

public class CreateCategoryCommand : IRequest<Unit>
{
    [Required] public string Name { get; set; }
    public string? Description { get; set; }
}