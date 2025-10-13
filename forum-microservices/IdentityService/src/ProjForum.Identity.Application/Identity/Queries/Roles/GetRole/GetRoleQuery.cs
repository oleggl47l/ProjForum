using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.Roles.GetRole;

public class GetRoleQuery : IRequest<Domain.Entities.Role>
{
    [Required] public string RoleId { get; init; } = null!;
}