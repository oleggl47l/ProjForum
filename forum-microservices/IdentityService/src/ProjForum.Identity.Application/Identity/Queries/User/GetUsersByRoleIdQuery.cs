using ProjForum.Identity.Domain.Models;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.User;

public class GetUsersByRoleIdQuery : IRequest<IEnumerable<UserModel>>
{
    public string RoleId { get; set; }
}