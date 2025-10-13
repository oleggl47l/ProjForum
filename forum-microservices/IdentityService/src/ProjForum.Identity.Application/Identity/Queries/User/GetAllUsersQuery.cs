using ProjForum.Identity.Domain.Models;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries.User;

public class GetAllUsersQuery: IRequest<IEnumerable<UserModel>>;