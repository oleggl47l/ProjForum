using System.ComponentModel.DataAnnotations;
using ProjForum.Identity.Domain.Models;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Role;

public class UpdateRoleCommand : IRequest<RoleModel>
{
    [Required] public string RoleId { get; set; } = null!;

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}