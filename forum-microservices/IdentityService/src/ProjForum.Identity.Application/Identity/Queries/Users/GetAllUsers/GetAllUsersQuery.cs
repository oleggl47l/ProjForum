using MediatR;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetAllUsers;

public class GetAllUsersQuery : IRequest<IEnumerable<UserModel>>;