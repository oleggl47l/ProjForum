using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Commands.Role;

public class DeleteRoleCommand : IRequest<Unit>
{
    [Required] public string RoleId { get; init; } = null!;
}