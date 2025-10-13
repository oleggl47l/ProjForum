using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Forum.Application.Forum.Commands.Categories;

public class DeleteCategoryCommand : IRequest<Unit>
{
    [Required] public Guid Id { get; set; }
}