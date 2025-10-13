using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.Roles.GetAllRoles;

public class GetAllRolesQuery : IRequest<List<Domain.Entities.Role>>;