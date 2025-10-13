using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Categories;

public class UpdateCategoryCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}