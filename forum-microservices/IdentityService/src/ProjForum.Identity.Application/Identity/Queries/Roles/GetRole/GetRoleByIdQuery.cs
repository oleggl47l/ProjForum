using MediatR;
using ProjForum.Identity.Application.DTOs.Role;

namespace ProjForum.Identity.Application.Identity.Queries.Roles.GetRole;

public record GetRoleByIdQuery(Guid Id) : IRequest<RoleDto?>;