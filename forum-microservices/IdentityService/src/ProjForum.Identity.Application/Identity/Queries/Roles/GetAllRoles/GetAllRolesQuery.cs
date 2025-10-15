using MediatR;
using ProjForum.Identity.Application.DTOs.Role;
using ProjForum.Identity.Domain.Identities;

namespace ProjForum.Identity.Application.Identity.Queries.Roles.GetAllRoles;

public record GetAllRolesQuery() : IRequest<IReadOnlyList<RoleDto>>;