using MediatR;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUsersByRoleId;

public class GetUsersByRoleIdQuery : IRequest<IEnumerable<UserModel>>
{
    public string RoleId { get; set; }
}