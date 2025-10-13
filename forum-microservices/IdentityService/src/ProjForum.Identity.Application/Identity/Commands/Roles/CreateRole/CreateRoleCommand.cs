using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.CreateRole;

public class CreateRoleCommand : IRequest<Unit>
{
    [Required] public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}