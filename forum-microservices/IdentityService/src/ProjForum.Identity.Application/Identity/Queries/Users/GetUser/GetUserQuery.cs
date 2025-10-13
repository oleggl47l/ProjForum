using MediatR;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUser;

public class GetUserQuery : IRequest<UserModel>
{
    public string UserId { get; set; }
}