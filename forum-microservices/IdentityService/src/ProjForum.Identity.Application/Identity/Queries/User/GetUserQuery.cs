using ProjForum.Identity.Domain.Models;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.User;

public class GetUserQuery : IRequest<UserModel>
{
    public string UserId { get; set; }
}