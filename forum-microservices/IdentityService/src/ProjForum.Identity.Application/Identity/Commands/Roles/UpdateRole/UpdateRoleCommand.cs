using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.UpdateRole;

public class UpdateRoleCommand : IRequest<RoleModel>
{
    [Required] public string RoleId { get; set; } = null!;

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}