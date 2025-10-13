using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.Role;

public class GetRoleQuery : IRequest<ProjForum.Identity.Domain.Entities.Role>
{
    [Required] public string RoleId { get; init; } = null!;
}