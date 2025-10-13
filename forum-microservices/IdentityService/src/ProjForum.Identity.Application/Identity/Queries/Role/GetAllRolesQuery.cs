using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.Role;

public class GetAllRolesQuery : IRequest<List<ProjForum.Identity.Domain.Entities.Role>>;
